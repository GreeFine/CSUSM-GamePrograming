#pragma once

#include <iostream>
#include <boost/asio.hpp>

#include "ISocket.hpp"

using boost::asio::ip::udp;

namespace Network
{

const constexpr int MAX_RECV_SIZE = 1024 * 20;

template <typename T, typename X>
class ABoostSocket : public ISocket<T, X>
{
public:
	ABoostSocket(std::shared_ptr<boost::asio::io_service> &p_io_service, int p_port) : socket_(*p_io_service, udp::endpoint(udp::v4(), p_port)){};
	ABoostSocket(const ABoostSocket &) = delete;
	ABoostSocket &operator=(const ABoostSocket &) = delete;
	virtual ~ABoostSocket(){};

	void async_receive_from(void (T::*p_memberfunction)(std::unique_ptr<Packet> &, std::shared_ptr<udp::endpoint> &), T *p_class)
	{
		std::shared_ptr<udp::endpoint> remote_endpoint(std::make_shared<udp::endpoint>());

		socket_.async_receive_from(
				boost::asio::buffer(recv_buffer_, MAX_RECV_SIZE), *remote_endpoint,
				std::bind(&ABoostSocket::handleReceive, this, std::placeholders::_1, std::placeholders::_2, p_memberfunction, p_class, remote_endpoint));
	}

	void async_send_to(const X *p_message, size_t p_size, const std::shared_ptr<udp::endpoint> &p_endpoint)
	{
		std::cout << "Sending to " << *p_endpoint;
		socket_.async_send_to(boost::asio::buffer(p_message, p_size), *p_endpoint,
													std::bind(&ABoostSocket::handleSend, this, std::placeholders::_1, std::placeholders::_2));
	}

	Packet receicve_from()
	{
		std::shared_ptr<udp::endpoint> remote_endpoint(std::make_shared<udp::endpoint>());

		auto size = socket_.receive_from(boost::asio::buffer(recv_buffer_, MAX_RECV_SIZE), *remote_endpoint);
		return std::move(Packet(recv_buffer_, size));
	}

	void send_to(const X *p_message, size_t p_size, const std::shared_ptr<udp::endpoint> &p_endpoint)
	{
		socket_.send_to(boost::asio::buffer(p_message, p_size), *p_endpoint);
	}

	int getPort() const
	{
		return socket_.local_endpoint().port();
	};

private:
	void handleReceive(const boost::system::error_code &error, std::size_t bytes_transferred,
										 void (T::*p_memberfunction)(std::unique_ptr<Packet> &, std::shared_ptr<udp::endpoint> &), T *p_class, std::shared_ptr<udp::endpoint> &p_remote_endpoint)
	{
		if (!error)
		{
			std::unique_ptr<Packet> received(std::make_unique<Packet>(recv_buffer_, bytes_transferred));
			(*p_class.*p_memberfunction)(received, p_remote_endpoint);
		}
		else
		{
			std::cerr << "Receive Error -> " << *p_remote_endpoint << " : " << error.message() << std::endl;
			std::unique_ptr<Packet> received(new Packet(-1));
			(*p_class.*p_memberfunction)(received, p_remote_endpoint);
			async_receive_from(p_memberfunction, p_class);
		}
	}

	void handleSend(const boost::system::error_code &error, std::size_t size)
	{
		std::cout << " " << size << " sended" << std::endl;
		if (error)
			std::cerr << "Send Error : " << error.message() << std::endl;
	}

	udp::socket socket_;
	char *recv_buffer_ = new char[MAX_RECV_SIZE];
};

} // namespace Network
