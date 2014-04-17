/****************************************************************************************
* FILENAME:           sendMessage.c                                                     *
* LOCAL DEPENDENCIES: None                                                              *
* COMPILATION:        Use Makefile, compile standalone executable with option -DMAIN    *
* EXECUTION:          If compiled with -DMAIN, ./sendMessage                            *
*                                                                                       *
* AUTHORS:      Thomas Gerstenberg, Chad Higgins, Kristin Makowski,                     *
*               Mark Parangue, Robert Paulino                                           *
* INSTITUTIONS: SDSU and SDG&E                                                          *
* SEMESTER:     SPRING 2014                                                             *
* PROJECT:      VICE VFD Controller and Monitor                                         *
*                                                                                       *
* This file houses the sendMessage function, used to send data messages back to the     *
* server.  Also, it holds helper functions error(), used to display error messages,     *
* and getServerIp(), which grabs the server ip stored in the file pointed to by IPFILE  *
****************************************************************************************/

//************************* NOTE: USE THE MAKEFILE TO COMPILE *************************//  
#include <stdio.h>
#include <stdlib.h>    
#include <string.h>    
#include <arpa/inet.h> //inet_addr
#include <fcntl.h>
#include <netinet/in.h>
#include <netdb.h>
#include <sys/socket.h>
#include <sys/stat.h>
#include <sys/types.h>
#include <unistd.h>    //write
#include <errno.h>
#include "sendMessage.h"

// **STANDALONE COMPILE SETTINGS FOR Raspi: gcc sendMessage.c -o sendMessage -DMAIN

#define IPFILE "/etc/vice/ip"
#define SERVER_PORT 9750 //sendmessage sends on 9750, we receive on 9700

int sendMessage(const char*, int);
char* getServerIp();
void error(char*);

#ifdef MAIN //compile as a standalone executable
int main(int argc, char* argv[])
{
    char* message;
    int messageLen;

    if (argc < 2)
    {
        error("Not enough arguments");
        return 0;
    }
    message = argv[1];
    messageLen = strlen(message);
    int retVal = sendMessage(message, messageLen);
    return retVal;
}
#endif

int sendMessage(const char* str, int len)
{
    if(len < 1)
        error("Message length < 1");

    if(len != strlen(str))
        printf("WARNING: length of string does not match length passed to function\n");

    int socket_desc;            // socket fd
    struct sockaddr_in  server; // server info to connect to

    //Create a TCP socket
    socket_desc = socket(AF_INET , SOCK_STREAM , 0);
    if (socket_desc == -1)
        error("Could not create socket\n");

    //Prepare the sockaddr_in structure
    server.sin_family   = AF_INET;
    server.sin_port     = htons(SERVER_PORT);   // Set port number to connect to
    server.sin_addr.s_addr = inet_addr(getServerIp());

    // Attempt connection
    if( connect(socket_desc, (struct sockaddr*)&server, sizeof(server)) < 0)
    {
        printf("sendMessage: Cannot connect to server\n");
        return 0;
    }
    else // Connection succeeded
    {
        printf("Pi-->Server: %s\n", str);  //Debug printf
        if(write(socket_desc, str, len) < 1)
            error("Writing to socket");
        // Close and reinitialize the socket descriptor
        if(close(socket_desc) < 0)
            error("Closing socket");
    }
    return 1;
}
 
/****************************************************************************
* error:                                                                    *
* Displays the error message passed and exits the program                   *
*                                                                           *
* <param msg> The string to be sent to the console for debugging purposes   * 
****************************************************************************/
void error(char* msg)
{
   printf("Error: %s; errno:%d\n", msg, errno);
   // exit(1);
}

/****************************************************************************
* getServerIp:                                                              *
* returns the server ip from the IPFILE                                     *
****************************************************************************/
char* getServerIp()
{
    FILE* fp;
    char* line = NULL;
    size_t len = 0;
    ssize_t read;

    fp = fopen(IPFILE, "r");
    if (fp == NULL)
        error("error opening file");
    if((read = getline(&line, &len, fp)) < 0)
        error("error getting IP address");

    fclose(fp);
    // replace \n at end of string with null char
    if(line[read-1] == '\n')
        line[read-1] = '\0';

    // printf("read: %d, server ip: <%s>\n",read, line);
    return line;
}