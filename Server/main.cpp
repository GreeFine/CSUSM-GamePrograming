#include <iostream>
#include "Server.hpp"

using namespace Network;

int main(int, char **)
{
    try
    {
        std::shared_ptr<boost::asio::io_service> io_service(new boost::asio::io_service);
        Server server(io_service);

        server.start();
        io_service->run();
    }
    catch (std::exception &e)
    {
        std::cerr << e.what() << std::endl;
        std::cin.ignore();
    }

    return 0;
}