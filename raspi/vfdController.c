/****************************************************************************************
* FILENAME: 	      vfdController.c 													*
* LOCAL DEPENDENCIES: sendMessage.c, sendMessage.h 										*
* COMPILATION:		  use Makefile 														*
* EXECUTION:  		  sudo ./controller 												*
* 																						*
* AUTHORS:      Thomas Gerstenberg, Chad Higgins, Kristin Makowski,                     *
*               Mark Parangue, Robert Paulino                                           *
* INSTITUTIONS: SDSU and SDG&E 															*
* SEMESTER: 	SPRING 2014 															*
* PROJECT: 		VICE VFD Controller and Monitor 										*
*																						*
* This is the main program that communicates with the VFD(Arduino) controller, as well	*
* as with the server.  It also calculates the frequency required for the desired rate 	*
* sent from the server. 																*
****************************************************************************************/

//************************* NOTE: USE THE MAKEFILE TO COMPILE *************************// 
#include <stdio.h>
#include <stdlib.h>    
#include <string.h>  //parsing
#include <unistd.h>  //read/write
#include <signal.h>  //sig_handler
#include <pthread.h> //for threading, compile with gcc option -lpthread
#include <sys/types.h>
#include <sys/socket.h>
#include <arpa/inet.h> 
#include <wiringPi.h>  		//xbee setup
#include <wiringSerial.h> 	//xbee UART setup
#include "sendMessage.h"

//****** Defines ******//

#define SERVER_PORT 9700 //sendmessage sends on 9750, we receive on 9700
// Serial port (xbee) baud rate
#define BAUD_RATE 9600

//Message buffer size. 256 should be way more than enough and can probably reduce to 128
#define MSG_SIZE 256
//Request packet string for pi-->vfd comm
#define DATAREQUEST "<REQ>"
//End of File string sent by server/vfd
#define EOFSTR "EOF"
//Delimiters for parsing the messages
#define DELIMS ",<>"
//Last character expected from the VFD
#define ENDCHAR '>'

#define MAX_FLOW 3.5 //TODO: Update when we get the real number at 60Hz
#define MIN_FLOW 1.0 //TODO: Update when we get the real number at 30Hz
#define MAX_FREQ 60
#define MIN_FREQ 30
//Threshold for when to update frequency. If the difference of the 
// actual and expected flow is > abs(FLOW_THRESH), frequency will be updated
#define FLOW_THRESH 0.15

//Converts milliseconds to microseconds
#define M_SEC(x) (x)*1000
//Wrapper for locking/unlocking mutex for readability
#define LOCK(mutex) 	pthread_mutex_lock(mutex)
#define UNLOCK(mutex) 	pthread_mutex_unlock(mutex)

//Flag functions
#define SETFLAG(x, flag) (x) |= (flag)
#define CLRFLAG(x, flag) (x) &= ~(flag)
#define CHKFLAG(x, flag) (x) & (flag)

//****** Function Declarations ******//

//Main's initialization function
void init(void);
//Signal handler to restart recv handler
void sig_handler(int signo);
//Listens to server and receives messages
void *receive_handler(void *);
//Determines if flow should be updated
void updateFlowRate(void);
//Gets the data packet from VFD/Arduino
int getDataFromSerial(void);
//Sends the data packet to VFD/Arduino
int sendPacketToVfd();
//Forwards the data received from serial to server
int sendDataToSever(void);

//Takes each delimited token and returns the ID and value
void parseToken(const char*, int*, char*);
int parseServerMessage(void);
int parseVfdMessage(void);

//****** Data Structures ******//

//Struct to save current vfd data.  Only updated from the VFD
struct vfdSettings
{
	float flowRate;
	float frequency;
	float temperature;
	float pressure;
	int VFDState;
}vfdSettings;
//Struct to save the update data from server/updateFlowRate
struct updateSettings
{
	float desiredFlowRate;
	float frequency;
	int VFDState;
	int updateFlag;
} updateSettings;

//****** Enums ******//

//Enum of the different update flags possible
enum
{
	NO_UPDATE 		  = 0,
	UPDATE_FREQ 	  = 1, // updated from this application
	UPDATE_STATE 	  = 2, // updated from server
	UPDATE_FREQ_STATE = 3, // not assigned -- used to check if both freq and state are flagged
	UPDATE_FLOW 	  = 4, // updated from server
};
//Enum of the possible VFD states
enum
{
	OFF = 0,
	ON_VFD,
	ON_NO_VFD
};
//Enum of the IDs for parsing
enum
{
	ID_FREQ = 0,
	ID_FLOW,
	ID_DESIREDFLOW,
	ID_TEMP,
	ID_PRESSURE,
	ID_STATE
};

/******************************** GLOBALS ********************************/
struct vfdSettings data;
struct updateSettings update;

char messageFromServer[MSG_SIZE];
char messageFromVfd[MSG_SIZE];

int serialFd = 0;
pthread_t thread_id;
pthread_mutex_t flagMutex;


/********************************* Main *********************************/
int main(int argc , char *argv[])
{
	init();
	//wait for receive to connect to server
	sleep(5);
	/*************************** Superloop ***************************/
	while(1)
	{	// Send request 
		if (data.VFDState == ON_VFD)
		{
			updateFlowRate();
		}
		sendPacketToVfd();
		// Wait ~1 second
		usleep(M_SEC(3000));	//925ms
		// Get data from serial and send to server
		if(getDataFromSerial())
		{
			sendDataToSever();
		}
	}

	pthread_join( thread_id, NULL);
	return 0;
}

/********************************************************************
* updateFlowRate:													*
* determines whether to change the frequency on the VFD to 			*
* match the desired flow rate 										*
********************************************************************/
void updateFlowRate()
{
	float prevFreq = update.frequency;	// Save the previous frequency

	// Save off desired flow in case server updates while in this function
	// If this happens, it will be updated on the next pass
	float currentDesiredFlow = update.desiredFlowRate;

	// If desired flow was updated via server
	// start with frequency based on lookup table
	if(CHKFLAG(update.updateFlag, UPDATE_FLOW))
	{
		//TODO: Change/update LUT for how frequency is first determined
		update.frequency = currentDesiredFlow*(60/3.5); //TEMPORARY
		printf("Update Flow set, flow: %f, freq: %f\n", currentDesiredFlow, update.frequency);

		// clear update flow flag
		LOCK(&flagMutex);
		CLRFLAG(update.updateFlag, UPDATE_FLOW);
		UNLOCK(&flagMutex);
	}
	else //adjust flow rate based on how off it is from actual
	{
		// float difference = 0;
		// difference = data.flowRate - currentDesiredFlow;
		// if (difference > FLOW_THRESH || difference < -FLOW_THRESH)
		// {
		// 	//TODO: Change how frequency is calculated
		// 	update.frequency -=  (difference*10); //TEMPORARY

		// 	if (update.frequency > MAX_FREQ)
		// 	{
		// 		update.frequency = MAX_FREQ;
		// 		printf("frequency calculated above %d\n", MAX_FREQ);
		// 	}
		// 	else if (update.frequency < MIN_FREQ)
		// 	{
		// 		update.frequency = MIN_FREQ;
		// 		printf("frequency calculated below %d\n", MIN_FREQ);
		// 	}
		// }
	}

	// update flag if frequency changed
	if (prevFreq != update.frequency)
	{
		LOCK(&flagMutex);
		SETFLAG(update.updateFlag, UPDATE_FREQ);
		UNLOCK(&flagMutex);
	}
}

/********************************************************************
* sendPacketToServer												*
* formulates the packet string and calls the sendMessage function	*
********************************************************************/
int sendDataToSever()
{
	char messageToSend[MSG_SIZE];
	int len = 0;
	len = sprintf(messageToSend, "<,F=%.3f,H=%.1f,T=%.3f,P=%.3f,VS=%d,><EOF>",
			data.flowRate,
			data.frequency,
			data.temperature,
			data.pressure,
			data.VFDState);

	if(sendMessage(messageToSend, len))
		return 1;
	else
		return 0;
}

/********************************************************************
* sendPacketToVfd:													*
* determines what packet string to send to the VFD and sends it		*
********************************************************************/
int sendPacketToVfd()
{
	static char messageToVfd[MSG_SIZE];
	int strlength = 0;

	// save off flag and clear frequency and state flags
	LOCK(&flagMutex);
	int flag = update.updateFlag;
	CLRFLAG(update.updateFlag, UPDATE_FREQ_STATE);
	UNLOCK(&flagMutex);

	//send message based on current flags and remove UPDATE FLOW flag
	switch(flag & ~UPDATE_FLOW)
	{
		case UPDATE_FREQ:
			strlength = sprintf(messageToVfd, "<H=%.1f>\n", update.frequency);
		break;
		case UPDATE_STATE:
			strlength = sprintf(messageToVfd, "<S=%d>\n", update.VFDState);
		break;
		case UPDATE_FREQ_STATE:
			strlength = sprintf(messageToVfd, "<H=%.1f,S=%d>\n",update.frequency, update.VFDState);
		break;
		default:
			strlength = sprintf(messageToVfd, "%s\n", DATAREQUEST);
		break;
	}
	// Clear flags for updating the frequency and state

	printf("\nPi-->VFD: %s", messageToVfd);
	write(serialFd, messageToVfd, strlength);
	return 1;
}

/********************************************************************
* getDataFromSerial:												*
* gets the data from the serial (xbee) modules 						*
* and calls parseVfdMessage 										*
********************************************************************/
int getDataFromSerial()
{
	int strlength = 0;
	int readlength = 0;
	// Check if data is available on the serial port
	if((strlength = serialDataAvail(serialFd)) > 0) // Check if data is available on the port
	{
		// Read data into messageFromVfd
		readlength = read(serialFd, &messageFromVfd, strlength);
		if(messageFromVfd[readlength - 1] != ENDCHAR)
		{
			printf("serial data unaligned! Flushing data in serial buffer. read:<%s>\n", messageFromVfd);
			serialFlush(serialFd);
			return 0;
		}
		else
		{
			messageFromVfd[readlength] = '\0'; //add null terminator to the end of the string
			printf("VFD-->Pi: %s\n", messageFromVfd);
			parseVfdMessage();
			return 1;
		}
	}
	else
	{
		printf("no data on serial\n");
		return 0; //no data available or error getting data
	}
}

/********************************* PARSING FUNCTIONS *********************************/

/********************************************************************
* parseVfdMessage:													*
* Parses the message from the VFD and updates the vfd data struct 	*
********************************************************************/
int parseVfdMessage()
{
	// Message formatting:
	//  	<F=%f,H=%f,T=%f,P=%f,VS=%d><EOF>
	//  	Flow,Freq,Temp,Pres,State
	int id = 0;
	char valstr[32];
	memset(valstr, 0, sizeof(valstr));

	//strtok breaks up the message into tokens, delimited by commas
	char* token = strtok(messageFromVfd, DELIMS);
	while(token != NULL)
	{
		if(strcmp(token, EOFSTR) != 0)
		{
			parseToken(token, &id, valstr);
			switch(id)
			{
				case ID_FLOW: //Flow
					data.flowRate = atof(valstr);
					break;
				case ID_FREQ: //Frequency
					data.frequency = atof(valstr);
					update.frequency = data.frequency;
					break;
				case ID_TEMP: //Temperature
					data.temperature = atof(valstr);
					break;
				case ID_PRESSURE: //Pressure
					data.pressure = atof(valstr);
					break;
				case ID_STATE: //Current VFD State
					data.VFDState = atoi(valstr);
					update.VFDState = data.VFDState;
					break;
				default:
					printf("UNKNOWN TAG ENCOUNTERED, TOKEN: ""%s""\n", token);
			}
		}
		token = strtok(NULL, DELIMS);
	}
	return 1;
}

/********************************************************************
* parseToken:														*
* Parses each token and returns the enum of the identifier idEnum	*
* and the char string value 										*
********************************************************************/
void parseToken(const char* token, int* idEnum, char* value)
{
	char id[4];
	memset(id, 0, sizeof(id));
	memset(value, 0, sizeof(value));
	int i = 0;
	int j = 0;
	//Parse until '=' for the id
	while(token[i] != '=')
	{
		id[i] = token[i];
		i++;
	}
	i++; //skip over the '=' sign
	//Parse until the end of the token
	while(token[i] != '\0')
	{
		value[j] = token[i];
		i++; j++;
	}

	if(!strcmp(id, "F"))
		*idEnum = ID_FLOW;
	else if (!strcmp(id, "H"))
		*idEnum = ID_FREQ;
	else if (!strcmp(id, "T"))
		*idEnum = ID_TEMP;
	else if (!strcmp(id, "P"))
		*idEnum = ID_PRESSURE;
	else if (!strcmp(id, "VS"))
		*idEnum = ID_STATE;
	else if (!strcmp(id, "DF"))
		*idEnum = ID_DESIREDFLOW;
	else
		*idEnum = -1;
}

/********************************************************************
* sig_handler:														*
* when the process gets a USR1 signal, kill and 					*
* restart the receive thread										*		
********************************************************************/
void sig_handler(int signo)
{
	if (signo == SIGUSR1)
    {
        printf("killing recv thread\n");
        if (pthread_cancel(thread_id) < 0)
            error("could not kill thread\n");

        pthread_join(thread_id, NULL);
        usleep(100);
        printf("starting thread\n");
        if( pthread_create(&thread_id, NULL, receive_handler, (void*)&thread_id) < 0)
            error("could not create recv thread\n");
    }
}

/********************************************************************
* init:																*
* initializes the program											*
********************************************************************/
void init()
{
	/* Initialize serial port for xbee comm */
	wiringPiSetup();
	if((serialFd = serialOpen("/dev/ttyAMA0", BAUD_RATE)) < 0)
		error("error opening serial port");

	//initialize the mutex
	pthread_mutex_init(&flagMutex, NULL);

	// Set up sig handler for USR1 signal (restart recv thread)
	if (signal(SIGUSR1, sig_handler) == SIG_ERR)
        error("error setting up the sig_handler\n");

    update.desiredFlowRate = MAX_FLOW;
    update.frequency = MAX_FREQ;
    update.VFDState = ON_VFD;
    // set all flags for first run
    LOCK(&flagMutex);
    update.updateFlag = UPDATE_FREQ_STATE | UPDATE_FLOW;
    UNLOCK(&flagMutex);

	// Initialize memory and flush remaining serial data
	serialFlush(serialFd);
    memset((void*)&data, 0, sizeof(data));
	memset((void*)&messageFromServer, 0, sizeof(messageFromServer));
	memset((void*)&messageFromVfd, 0, sizeof(messageFromVfd));

	// Create thread to monitor the socket and receive messages
	if( pthread_create(&thread_id, NULL, receive_handler, (void*)&thread_id) < 0)
		error("could not create recv thread");
}

//======================================================================================================//
// 										RECEIVE THREAD FUNCTIONS										//


/********************************************************************
* receive_handler:													*
* thread that handles all incoming communication from the server 	*
********************************************************************/
void *receive_handler(void *arg)
{
	printf("receive thread created\n");
	int socket_desc;
	struct sockaddr_in server;

	//Create socket
	socket_desc = socket(AF_INET , SOCK_STREAM , 0);
	if (socket_desc == -1)
		error("Could not create socket");

	//Prepare the sockaddr_in structure
	server.sin_family = AF_INET;
	server.sin_port = htons(SERVER_PORT);
	server.sin_addr.s_addr = inet_addr(getServerIp());

	int read_size = 0;

	/****************** Receiving superloop ******************/
	while(1)
	{
		printf("Connecting to server...\n");
		while( connect(socket_desc, (struct sockaddr*)&server, sizeof(server)) < 0 )
		{
			printf("Connection failed, retry in 5 seconds\n");
			sleep(5);
		}
		printf("Connected!\n");
		
		// Continuously read from server while connection is active
		while( (read_size = recv(socket_desc , messageFromServer , MSG_SIZE , 0)) > 0 )
		{
			messageFromServer[read_size] = '\0';
			printf("Server-->Pi: %s\n", messageFromServer);
			parseServerMessage();
		}

		// Server disconnection
		if(read_size == 0)
		{
			printf("Server disconnected\n");
		}
		else if(read_size == -1)
		{
			error("recv failed\n");
		}

		// Close then reinit the socket and attempt reconnect to server
		close(socket_desc);
		socket_desc = socket(AF_INET , SOCK_STREAM , 0);
	}
	return 0;
}

/********************************************************************
* parseServerMessage: parses the current message and 				*
* updates the values sent from the Server 							*
********************************************************************/
int parseServerMessage()
{
	// Message formatting:
	//  	<F=%d,H=%d,T=%d,P=%d,VS=%d><EOF>
	//  	Flow,Freq,Temp,Pres,State
	int id = 0;
	char valstr[32];
	memset(valstr, 0, sizeof(valstr));

	//strtok breaks up the message into tokens, delimited by commas
	char* token = strtok(messageFromServer, DELIMS);
	while(token != NULL)
	{
		if(strcmp(token, EOFSTR) != 0)
		{
			parseToken(token, &id, valstr);
			switch(id)
			{
				case ID_STATE: //Current VFD State
					update.VFDState  = atoi(valstr);
					// update flag
    				LOCK(&flagMutex);
    				SETFLAG(update.updateFlag, UPDATE_STATE);
    				UNLOCK(&flagMutex);
					break;
				case ID_DESIREDFLOW:
					update.desiredFlowRate = atof(valstr);
					// update flag
    				LOCK(&flagMutex);
					SETFLAG(update.updateFlag, UPDATE_FLOW);
    				UNLOCK(&flagMutex);
					break;
				default:
					printf("UNKNOWN TAG ENCOUNTERED, TOKEN: ""%s""\n", token);
			}
		}
		token = strtok(NULL, DELIMS);
	}
	return 1;
}