#include <cstdlib>
#include <cstring>
#include <iostream>
#include <thread>
#include <boost/asio.hpp>

#include "Packet.hpp"

using boost::asio::ip::udp;
using namespace Network;

udp::socket *s;
boost::asio::io_service io_service;
udp::resolver resolver(io_service);

const int b_size = 1024 * 20;
char *buffer = new char[b_size];
const char ip[] = "51.75.141.132";
// const int timer = 100;
const int timerR = 500;
udp::endpoint endpoint;

void handleReceive(const boost::system::error_code &error,
				   std::size_t size)
{
	if (error)
		std::cerr << error.message() << std::endl;
	std::cout << "Received(" << size << ") :" << buffer << std::endl;
}

void conection()
{
	std::cout << "Automatic mode" << '\n';
	std::string request1 = "CO";
	std::string request2 = "Play";
	std::this_thread::sleep_for(std::chrono::milliseconds(timerR));

	endpoint = *resolver.resolve({udp::v4(), ip, "1024"});
	std::cout << "Sending CO" << std::endl;
	Packet packet(0);
	packet.ack = 1;
	packet.serialize();
	s->send_to(boost::asio::buffer(packet.data(), packet.size()), endpoint);
	std::this_thread::sleep_for(std::chrono::milliseconds(timerR));

	std::cout << "Waiting for port... ";
	std::cout.flush();
	const int rcv_size = s->receive(boost::asio::buffer(buffer, b_size));
	Packet packetR(buffer, rcv_size);
	packetR.deserialize();
	std::cout << "Got : " << packetR.messages[0].buffer_ << std::endl;
	std::this_thread::sleep_for(std::chrono::milliseconds(timerR));

	std::cout << "Sending Play... ";
	Packet packet2(0);
	packet2.ack = 2;
	packet2.messages.emplace_back(0, "GreeFine", 9);
	packet2.serialize();
	endpoint = *resolver.resolve({udp::v4(), ip, packetR.messages[1].buffer_});
	s->send_to(boost::asio::buffer(packet2.data(), packet2.size()), endpoint);
	std::cout << "Done." << std::endl;
	std::this_thread::sleep_for(std::chrono::milliseconds(timerR));
}

void automatic(int p_loss)
{
	conection();
	std::cout << "IG MODE" << std::endl;

	unsigned int current_packetID = 0;
	unsigned int send_packetID = 0;

	unsigned int ack = 0;
	unsigned int ack_mask = 0;
	while (420)
	{
		//IF coorupted got fail on server side
		auto size = s->receive(boost::asio::buffer(buffer, b_size));
		std::cout << "Receiv (" << size << ") : ";
		std::cout.flush();
		if (rand() % 100 > p_loss)
		{
			Packet packet_in(buffer, size);
			packet_in.deserialize();
			if (packet_in.messages.size() > 0)
			{
				std::string str(packet_in.messages[0].buffer_, packet_in.messages[0].size_);
				std::cout << "id : " << packet_in.id() << str << std::endl;
			}
			if (packet_in.id() < current_packetID)
			{
				std::cout << "Discarded " << packet_in.id() << '/' << current_packetID << std::endl;
			}
			else if (packet_in.id() > current_packetID)
				current_packetID = packet_in.id();

			Packet packet_out(++send_packetID);
			std::cout << "Recv Packet ack : " << packet_in.ack << std::endl;
			//for (auto & it : packet_in.messages)
			//    if (packet_in.ack > ack)
			//    {
			//        ack_mask = (ack_mask >> (it.id_ - ack));
			//        ack = packet_in.ack;
			//    }
			ack = packet_in.ack;
			ack_mask = 0;
			for (auto &it : packet_in.messages)
			{
				std::cout << "Recieve Msg:" << it.id_ << '\n';
				ack_mask |= (1 << (it.id_ - ack));
			}
			std::cout << std::endl;
			packet_out.ack = ack;
			packet_out.ack_mask = ack_mask;
			packet_out.serialize();
			s->send_to(boost::asio::buffer(packet_out.data(), packet_out.size()), endpoint);
		}
		else
		{
			std::cout << "Packet Lost" << std::endl;
		}
	}
}

int main(int ac, char *av[])
{

	int p_loss = -1;

	try
	{
		s = new udp::socket(io_service, udp::endpoint(udp::v4(), 0));

		std::cout << "Starting with : ";
		if (ac == 1)
		{
			//AUTO MODE
			automatic(p_loss);
			return 0;
		}
		else if (ac == 2)
		{
			std::cout << "port : " << av[1] << std::endl;
			endpoint = *resolver.resolve({udp::v4(), "192.168.0.190", av[1]});
		}
		else
		{
			std::cout << "address : " << av[1] << ", port : " << av[2] << std::endl;
			endpoint = *resolver.resolve({udp::v4(), av[1], av[2]});
		}

		return 0;
	}
	catch (std::exception &e)
	{
		std::cerr << "Exception: " << e.what() << "\n";
		std::cin.ignore();
	}

	return 0;
}
