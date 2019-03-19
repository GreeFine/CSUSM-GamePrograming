#include <thread>
#include <iostream>
#include "GameRoom.hpp"

using namespace Network;

const unsigned int GameLoopSpeed = 10;

GameRoom::GameRoom(std::shared_ptr<boost::asio::io_service> &p_io_service) : ISocket_(*new ABoostSocket<GameRoom, char>(p_io_service, 0)), port_(std::to_string(ISocket_.getPort()))
{
}

void GameRoom::start()
{
	std::cout << "GameRoom start." << std::endl;

	ISocket_.async_receive_from(&GameRoom::handleReceive, this);
	while (1)
	{
		sendGameState();
		alivePlayers();
		std::this_thread::sleep_for(std::chrono::milliseconds(GameLoopSpeed));
	}
}

bool GameRoom::isFull()
{
	return (players_.size() >= n_players);
}

void GameRoom::handleReceive(std::unique_ptr<Packet> &received, std::shared_ptr<udp::endpoint> &p_endpoint)
{
	received->deserialize();

	std::cout << "Rcv message (" << received->id() << ") : "
						<< " From : " << *p_endpoint << std::endl;
	const auto &player = std::find(players_.begin(), players_.end(), *p_endpoint);

	if (!isFull() && player == players_.end())
	{
		if (received->messages.size() > 0)
		{
			std::string p_name(received->messages.front().buffer_, received->messages.front().size_);
			players_.emplace(players_.size(), Player(players_.size(), p_endpoint, p_name));
		}
		else
			std::cerr << "User connected without a name" << std::endl;
		//std::cout << "New player in gameroom !, Currently : " << players_.size() << std::endl;
	}
	else if (player != players_.end())
	{
		player->second.openPacket(*received);
	}
	ISocket_.async_receive_from(&GameRoom::handleReceive, this);
}

void GameRoom::updateGameState()
{
	//TODO:
}

void GameRoom::alivePlayers()
{
	for (auto it = players_.begin(); it != players_.end();)
		if (!it->second.alive())
		{
			std::cout << "Player " << it->second.id() << " disconnected" << std::endl;
			std::lock_guard<std::mutex> lock(mutex_);
			it = players_.erase(it);
		}
		else
		{
			++it;
		}
}

std::string message = "||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||";
void GameRoom::sendGameState()
{
	std::lock_guard<std::mutex> lock(mutex_);
	for (auto &it : players_)
	{
		auto new_end = std::to_string(rand());
		message.replace(message.size() - new_end.size(), new_end.size(), new_end.c_str());
		it.second.newMessage(message.c_str(), message.length()); //DEBUG
		auto packet = it.second.newPacket();
		packet.serialize();
		try
		{
			std::cout << "sendGameState -- ACK: " << packet.ack << std::endl;
			std::cout << "sendGameState -- Size: " << packet.size() << std::endl;
			std::cout << "sendGameState -- Mask: " << packet.ack_mask << std::endl;
			ISocket_.async_send_to(packet.data(), packet.size(), it.second.enpoint_);
		}
		catch (std::exception &e)
		{
			std::cerr << "Error sending to " << it.second.id() << " : " << e.what();
			players_.erase(it.first);
		}
	}
}
