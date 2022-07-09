#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <netdb.h>

#include "hacking.h"
#include "hacking-network.h"

void ip_lookup(int argc, char *hostname)
{
    struct hostent *host_info;
    struct in_addr *address;

    host_info = gethostbyname(hostname);
    if (host_info == NULL)
    {
        printf("Couldn't lookup %s\n", hostname);
    }
    else
    {
        address = (struct in_addr *)(host_info->h_addr);
        printf("%s has address %s\n", hostname, inet_ntoa(*address));
    }
}

void webserver_id(int argc, char *hostname)
{
    int sockfd;
    struct hostent *host_info;
    struct sockaddr_in target_addr;
    unsigned char buffer[4096];

    if ((host_info = gethostbyname(hostname)) == NULL)
        fatal("looking up hostname");

    if ((sockfd = socket(PF_INET, SOCK_STREAM, 0)) == -1)
        fatal("in socket");

    target_addr.sin_family = AF_INET;
    target_addr.sin_port = htons(80);
    target_addr.sin_addr = *((struct in_addr *)host_info->h_addr);
    memset(&(target_addr.sin_zero), '\0', 8); // zero the rest of the struct

    if (connect(sockfd, (struct sockaddr *)&target_addr, sizeof(struct sockaddr)) == -1)
        fatal("connecting to target server");

    send_string(sockfd, "HEAD / HTTP/1.0\r\n\r\n");

    while (recv_line(sockfd, buffer))
    {
        if (strncasecmp(buffer, "Server:", 7) == 0)
        {
            printf("The web server for %s is %s\n", hostname, buffer + 8);
            exit(0);
        }
    }
    printf("Server line not found\n");
    exit(1);
}

int main(int argc, char *argv[])
{

    if (argc < 2)
    {
        printf("Usage: <hostname>\n");
        exit(1);
    }

    ip_lookup(argc, argv[1]);
    webserver_id(argc, argv[1]);
}