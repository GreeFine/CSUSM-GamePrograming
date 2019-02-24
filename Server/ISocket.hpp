#pragma once

#include <boost/asio.hpp>

#include "Packet.hpp"

using boost::asio::ip::udp;

namespace Network {

	template<typename T, typename X>
	class ISocket
	{
	public:
		virtual void async_receive_from(void(T::*p_memberfunction)(std::unique_ptr<Packet>&, std::shared_ptr<udp::endpoint>&), T *p_class) = 0;
		virtual void async_send_to(const X* p_message, size_t p_size, const std::shared_ptr<udp::endpoint>& p_endpoint) = 0;
		virtual Packet receicve_from() = 0;
		virtual void send_to(const X* p_message, size_t p_size, const std::shared_ptr<udp::endpoint>& p_endpoint) = 0;
		virtual int getPort() const = 0;
	};

}
