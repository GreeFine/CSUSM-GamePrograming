
 ///Created by greefine on 1/12/18.

#pragma once

#include <unordered_map>
#include <mutex>

#include "ABoostSocket.hpp"
#include "Player.hpp"

namespace Network {

	const constexpr int n_players = 4;

	class GameRoom {
	public:
		GameRoom(std::shared_ptr<boost::asio::io_service>& p_io_service);

		void	start();
		bool	isFull();

	private:
		void	handleReceive(std::unique_ptr<Packet>& received, std::shared_ptr<udp::endpoint>& p_endpoint);
		void	updateGameState();
		void	alivePlayers();
		void	sendGameState();

	private:
		ISocket<GameRoom, char>& ISocket_;
		std::unordered_map<int, Player> players_;
		int		game_frame = 0;
		std::mutex mutex_;

	public:
		const std::string port_;

	};
}