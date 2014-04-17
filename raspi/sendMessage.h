/****************************************************************************************
* FILENAME: 	      sendMessage.h 													*
* LOCAL DEPENDENCIES: None			 			 										*
* COMPILATION:		  Use Makefile, compile standalone executable with option -DMAIN	*
* EXECUTION:  		  If compiled with -DMAIN, ./sendMessage							*
* 																						*
* AUTHORS:      Thomas Gerstenberg, Chad Higgins, Kristin Makowski,                     *
*               Mark Parangue, Robert Paulino                                           *
* INSTITUTIONS: SDSU and SDG&E 															*
* SEMESTER: 	SPRING 2014 															*
* PROJECT: 		VICE VFD Controller and Monitor 										*
*																						*
* This is the header file for sendMessage.c.  Nothing else to see here					*
****************************************************************************************/

//************************* NOTE: USE THE MAKEFILE TO COMPILE *************************// 
#ifndef SENDMESSAGE_H
#define SENDMESSAGE_H

int sendMessage(const char* str, int len);
void error(char* msg);
char* getServerIp();

#endif 