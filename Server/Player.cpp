#include "Player.hpp"

using namespace Network;

Packet Player::newPacket()
{
	Packet packet(++packet_n_);
	packet.ack = cur_ack_;
	{
		std::lock_guard<std::mutex> lock(mutex_);
		for (auto &it : message_container_)
			packet.messages.push_back(it.second);
	}
	return std::move(packet);
}

void Player::newMessage(char *p_buffer, size_t p_size)
{
	char *buffer_cpy = new char[p_size];
	memcpy(buffer_cpy, p_buffer, p_size);
	message_container_.emplace(current_message_id_, Message(current_message_id_, buffer_cpy, p_size));
	++current_message_id_;
}

void Player::newMessage(const char *p_buffer, size_t p_size)
{
	char *buffer_cpy = new char[p_size];
	memcpy(buffer_cpy, p_buffer, p_size);
	message_container_.emplace(current_message_id_, Message(current_message_id_, buffer_cpy, p_size));
	++current_message_id_;
}

void Player::openPacket(Packet &p_packet)
{
	if (p_packet.id() > last_recv_id_)
	{
		ack_depile(p_packet);
		last_recv_id_ = p_packet.id();
	}
}

const short ack_size = sizeof(Packet::ack_mask) * 8;
void Player::ack_depile(Packet &p_packet)
{
	auto a_ack = p_packet.ack;
	bool locked = false; //TODO best metodo ?

	for (auto i = 0; i < ack_size; ++i)
	{
		if (p_packet.ack_mask & (1 << (i)))
		{
			//std::cout << "Packet " << a_ack + i << " : received" << std::endl;
			std::lock_guard<std::mutex> lock(mutex_);
			auto it = message_container_.find(a_ack + i);
			if (it != message_container_.end())
			{
				delete it->second.buffer_;
				message_container_.erase(it);
			}
		}
		else
		{
			if (cur_ack_ <= a_ack + i && !locked)
			{
				cur_ack_ = a_ack + i;
				locked = true;
			}
		}
	}
}

bool Player::alive() const
{
	return (cur_ack_ + timeout > current_message_id_);
}