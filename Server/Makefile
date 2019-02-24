##
## Makefile for UdpGameServer in /home/greefine/Projects/cpp_rtype/Server
##
## Made by Raphael Chriqui
## Login   <chriqu_r@epitech.net>
##
## Started on  Sat Mar 26 16:52:31 2016 Raphael Chriqui
## Last update Thu Aug  4 00:50:57 2016 Raphaël Chriqui
##

CPP=		clang++

CPPFLAGS+= -Wextra -Wall -g3 -Werror

LDFLAGS+= -lpthread

NAME=		UdpGameServer

SRCS=		GameRoom.cpp main.cpp Packet.cpp Player.cpp Server.cpp

OBJS=		$(SRCS:.cpp=.o)

RM=		rm -f

all:		$(NAME)

$(NAME):	$(OBJS)
		$(CPP) -o $(NAME) $(OBJS) $(LDFLAGS) $(CPPFLAGS)

clean:
		$(RM) $(OBJS)
		$(RM) $(GCH)

fclean:		clean
		$(RM) $(NAME)

re:		fclean all

test:
	$(CPP) -o tester UdpTester.cpp $(LDFLAGS) $(CPPFLAGS)

.PHONY: all clean fclean re test
