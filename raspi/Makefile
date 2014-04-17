all: controller sendMessage
	@echo Done.
controller: vfdController.c sendMessage.c sendMessage.h
	@echo Compiling VFD Controller...
	@gcc -o controller vfdController.c sendMessage.c -I. -Wall -lpthread -lwiringPi

sendMessage: sendMessage.c sendMessage.h
	@echo Compiling Send Message...
	@gcc -o sendMessage sendMessage.c -I. -Wall -DMAIN

clean:
	@rm -rf controller
	@rm -rf sendMessage
	@echo Cleaned.