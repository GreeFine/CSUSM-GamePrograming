//
// Created by daine on 1/11/18.
//

#pragma once

#include <thread>
#include <iostream>
#include <unordered_map>

#include "ABoostSocket.hpp"
#include "GameRoom.hpp"

namespace Network {

	enum e_messages { ConnectionSuccess, ReconnectSuccess, InvalidRequest, GameRoomFull, PlayerAlreadyConnected, PlayerNotFound, Success };
	const std::string messages[]{ "Connection success", "Reconnect success", "Invalid Request",  "GameRoom Full",  "Player already connected", "Player not found", "Success" };
	
	class Server {
	public:
		Server(std::shared_ptr<boost::asio::io_service>& p_io_service);
		Server(const Server&) = delete;
		Server& operator=(const Server &) = delete;
		~Server() = default;
		void start();

	private:
		void handleReceive(std::unique_ptr<Packet> & received, std::shared_ptr<udp::endpoint> & p_endpoint);
		void connect(std::shared_ptr<udp::endpoint> & p_endpoint);
		void reconnect(std::unique_ptr<Packet>& received, std::shared_ptr<udp::endpoint> & p_endpoint);
		void connectTo(std::unique_ptr<Packet>& received, std::shared_ptr<udp::endpoint> & p_endpoint);
		void initGameRoom(std::string & name);
		void listGameRooms(std::shared_ptr<udp::endpoint>& p_endpoint);
		void initPlayer(std::unique_ptr<Packet> & received, std::shared_ptr<udp::endpoint>& p_endpoint);

	private:
		std::shared_ptr<boost::asio::io_service> io_service_;
		ISocket<Server, char>& ISocket_;
		std::map<std::string, GameRoom *> game_rooms_;
		std::map<std::string, int> players_;
	};

}