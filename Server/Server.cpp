//
// Created by daine on 1/11/18.
//

#include "Server.hpp"
#include "GameRoom.hpp"

using namespace Network;

Server::Server(std::shared_ptr<boost::asio::io_service> &p_io_service)
		: io_service_(p_io_service), ISocket_(*new ABoostSocket<Server, char>(p_io_service, 1024))
{
	std::cout << "Server Start" << std::endl;
}

void Server::start()
{
	ISocket_.async_receive_from(&Server::handleReceive, this);
}

void Server::handleReceive(std::unique_ptr<Packet> &received, std::shared_ptr<udp::endpoint> &p_endpoint)
{
	received->deserialize();
	std::cout << "Receive (" << received->id() << ") ack : " << received->ack << " From : " << *p_endpoint << std::endl;

	if (received->ack == Message::INIT)
		initPlayer(received, p_endpoint);
	else if (received->ack == Message::CONNECT)
		connect(p_endpoint);
	else if (received->ack == Message::RECONNECT)
		reconnect(received, p_endpoint);
	else if (received->ack == Message::CONNECTTO)
		connectTo(received, p_endpoint);
	else if (received->ack == Message::GRLIST)
		listGameRooms(p_endpoint);

	ISocket_.async_receive_from(&Server::handleReceive, this);
}

void Server::initPlayer(std::unique_ptr<Packet> &received, std::shared_ptr<udp::endpoint> &p_endpoint)
{
	Packet packet;
	packet.ack = Network::Message::STATUS::INIT;

	std::string player_name(received->messages.front().buffer_, received->messages.front().size_);
	std::cout << "Player : " << player_name << " connected." << std::endl;
	if (players_.find(player_name) == players_.end())
	{
		packet.messages.emplace_back(1, messages[Success].c_str(), messages[Success].size());
		players_.emplace(player_name, 0);
	}
	else
	{
		packet.messages.emplace_back(-1, messages[PlayerAlreadyConnected].c_str(), messages[PlayerAlreadyConnected].size());
		std::cout << "Player " << player_name << " already connected" << std::endl;
	}
	packet.serialize();
	ISocket_.async_send_to(packet.data(), packet.size(), p_endpoint);
}

void Server::initGameRoom(std::string &name)
{
	std::cout << "New GameRoom !" << std::endl;
	game_rooms_.emplace(name, new GameRoom(io_service_));
	new std::thread(&GameRoom::start, game_rooms_[name]);
}

void Server::listGameRooms(std::shared_ptr<udp::endpoint> &p_endpoint)
{
	Packet p;
	p.ack = Network::Message::STATUS::GRLIST;

	p.messages.emplace_back(1, messages[Success].c_str(), messages[Success].size());
	for (auto it : game_rooms_)
	{
		std::string name(it.first + " : " + it.second->port_);
		p.messages.emplace_back(1, name);
	}
	p.serialize();
	ISocket_.async_send_to(p.data(), p.size(), p_endpoint);
}

void Server::connect(std::shared_ptr<udp::endpoint> &p_endpoint)
{
	Packet p;
	p.ack = Network::Message::STATUS::CONNECT;
	for (auto it : game_rooms_)
		if (!it.second->isFull())
		{
			p.messages.emplace_back(1, messages[ConnectionSuccess].c_str(), messages[ConnectionSuccess].size());
			p.messages.emplace_back(1, it.second->port_.c_str(), it.second->port_.size());
			break;
		}
	if (p.messages.size() == 0)
	{
		std::string room_name;
		for (int i = 0; i < std::numeric_limits<int>::max(); ++i)
		{
			room_name = std::to_string(i);
			if (game_rooms_.find(room_name) == game_rooms_.end())
			{
				initGameRoom(room_name);
				p.messages.emplace_back(-1, messages[ConnectionSuccess].c_str(), messages[ConnectionSuccess].size());
				p.messages.emplace_back(-1, game_rooms_[room_name]->port_.c_str(), game_rooms_[room_name]->port_.size());
				break;
			}
		}
	}
	p.serialize();
	ISocket_.async_send_to(p.data(), p.size(), p_endpoint);
}

void Server::reconnect(std::unique_ptr<Packet> &received, std::shared_ptr<udp::endpoint> &p_endpoint)
{
	Packet p;
	p.ack = Network::Message::STATUS::RECONNECT;

	if (received->messages.size() > 0)
	{
		std::string player_name(received->messages.front().buffer_, received->messages.front().size_);
		auto player = players_.find(player_name);
		if (player == players_.end())
			p.messages.emplace_back(-1, messages[PlayerNotFound].c_str(), messages[PlayerNotFound].size());
		else
			p.messages.emplace_back(1, std::to_string(player->second).c_str(), std::to_string(player->second).size());
	}
	else
		p.messages.emplace_back(-1, messages[InvalidRequest].c_str(), messages[InvalidRequest].size());
	p.serialize();
	ISocket_.async_send_to(p.data(), p.size(), p_endpoint);
}

void Server::connectTo(std::unique_ptr<Packet> &received, std::shared_ptr<udp::endpoint> &p_endpoint)
{
	Packet p;
	p.ack = Network::Message::STATUS::CONNECTTO;
	if (received->messages.size() > 0)
	{
		std::string game_name(received->messages.front().buffer_, received->messages.front().size_);
		auto gm = game_rooms_.find(game_name);
		if (gm == game_rooms_.end())
		{
			initGameRoom(game_name);
			p.messages.emplace_back(1, game_rooms_[game_name]->port_.c_str(), game_rooms_[game_name]->port_.size());
		}
		else if (!gm->second->isFull())
		{
			p.messages.emplace_back(1, messages[Success].c_str(), messages[Success].size());
			p.messages.emplace_back(1, game_rooms_[game_name]->port_.c_str(), game_rooms_[game_name]->port_.size());
		}
		else
			p.messages.emplace_back(-1, messages[GameRoomFull].c_str(), messages[GameRoomFull].size());
	}
	else
		p.messages.emplace_back(-1, messages[InvalidRequest].c_str(), messages[InvalidRequest].size());
	p.serialize();
	ISocket_.async_send_to(p.data(), p.size(), p_endpoint);
}