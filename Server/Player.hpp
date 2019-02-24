#pragma once

#include <deque>
#include <mutex>

#include "ABoostSocket.hpp"
#include "Packet.hpp"

namespace Network
{

const constexpr int timeout = (300 * 1);

class Player
{
public:
	Player(size_t p_id, std::shared_ptr<udp::endpoint> &p_enpoint, std::string &p_name) : enpoint_(p_enpoint), name_(p_name), id_(p_id){};
	Player(Player &&o) noexcept : enpoint_(std::move(o.enpoint_)), id_(std::move(o.id_)){};
	Player(const Player &) = delete;
	Player &operator=(const Player &) = delete;

	inline int id() const { return id_; };
	Packet newPacket();
	void openPacket(Packet &p_packet);
	void newMessage(const char *p_buffer, size_t p_size);
	void newMessage(char *p_buffer, size_t p_size);
	bool alive() const;

public:
	std::shared_ptr<udp::endpoint> enpoint_;
	std::map<int, Message> message_container_;

private:
	void ack_depile(Packet &p_packet);

	const std::string name_;
	const size_t id_;
	size_t packet_n_ = 0;
	unsigned int last_recv_id_ = 0;
	unsigned int current_message_id_ = 0;
	unsigned int cur_ack_ = 0;
	std::mutex mutex_;
};

inline bool operator==(const Player &left, const Player &right) { return left.id() == right.id(); }
inline bool operator==(std::pair<const int, Player> &left, const udp::endpoint &right) { return *(left.second.enpoint_) == right; }

} // namespace Network