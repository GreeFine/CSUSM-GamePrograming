#pragma once

#include <vector>
#include <array>

//TODO: remove
#include <iostream>
#include <cstring>

namespace Network
{

struct Message
{

	Message(int p_id, const char *p_buffer, int p_size) : id_(p_id), size_(p_size), buffer_(p_buffer){};
	Message(int p_id, std::string &p_str) : id_(p_id)
	{
		size_ = p_str.size();
		char *newBuffer_ = new char[size_];
		std::memcpy(newBuffer_, p_str.c_str(), size_);
		buffer_ = newBuffer_;
		toDelete = true;
	};

	Message() = default;

	~Message()
	{
		if (toDelete)
			delete buffer_;
	};

	enum STATUS
	{
		INIT = 0,
		CONNECT,
		CONNECTTO,
		RECONNECT,
		GRLIST,
		INGAME
	};
	int id_;
	unsigned int size_;
	const char *buffer_;
	bool toDelete = false;
};

const constexpr int PACKET_SIZE = 1024 * 20;

class Packet
{
public:
	Packet() = default;
	Packet(int p_packet_id) : id_(p_packet_id){};
	Packet(char *p_buffer, int p_size) : size_(p_size)
	{
		memcpy(buffer_.data(), p_buffer, p_size);
	};
	Packet(Packet &&o) noexcept : ack(o.ack),
																ack_mask(o.ack_mask),
																messages(std::move(o.messages)),
																id_(std::move(o.id_)),
																buffer_(std::move(o.buffer_)){};
	Packet(const Packet &) = delete;
	Packet &operator=(const Packet &) = delete;

	inline unsigned int id() { return id_; };
	inline unsigned int size() { return size_; };
	inline const char *data() { return buffer_.data(); };

public:
	unsigned int ack = 0;
	unsigned int ack_mask = 0;
	std::vector<Message> messages;

	void serialize()
	{
		writeInBuffer(id_);
		writeInBuffer(ack);
		writeInBuffer(ack_mask);

		for (auto &m : messages) //STOP if 1024 >
		{
			if (size_ + m.size_ > PACKET_SIZE)
				return;
			writeInBuffer(m.id_);
			writeInBuffer(m.size_);
			writeInBuffer(m.buffer_, m.size_);
		}
	}

	void deserialize()
	{
		readInBuffer(id_);
		readInBuffer(ack);
		readInBuffer(ack_mask);

		while (pos_read < size_ && !invalid_)
		{
			Message m;
			readInBuffer(m.id_);
			readInBuffer(m.size_);
			readInBuffer(m.buffer_, m.size_);
			messages.push_back(m);
		}

		if (invalid_)
			std::cerr << "Corupted Packet" << std::endl;
	}

private:
	template <typename T>
	void writeInBuffer(T &p_i)
	{
		if (size_ < PACKET_SIZE)
		{
			*(reinterpret_cast<T *>(buffer_.data() + size_)) = p_i;
			size_ += sizeof(T);
		}
	}

	template <typename T>
	void writeInBuffer(T &p_i, unsigned int p_size)
	{
		std::memcpy(buffer_.data() + size_, p_i, p_size);
		size_ += p_size;
	}

	template <typename T>
	void readInBuffer(T &p_obj)
	{
		if (pos_read + sizeof(T) > size_ || invalid_)
			invalid_ = true;
		else
		{
			p_obj = *(reinterpret_cast<T *>(buffer_.data() + pos_read));
			pos_read += sizeof(T);
		}
	}

	template <typename T>
	void readInBuffer(T &p_obj, unsigned int size)
	{
		if (pos_read + size > size_ || invalid_)
			invalid_ = true;
		else
		{
			p_obj = (reinterpret_cast<T>(buffer_.data() + pos_read));
			pos_read += size;
		}
	}

	bool invalid_ = false;
	unsigned int id_ = -1;
	unsigned int size_ = 0;
	unsigned int pos_read = 0;
	std::array<char, PACKET_SIZE> buffer_;
};

} // namespace Network