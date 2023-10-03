#include "connection.h"
#include "ui_connection.h"
#include <QFile>
#include <QJsonDocument>
#include <QTimer>



std::unordered_map<QString, TCPMsg*> TCPMsg::messages{};
//const QString Connection::hostIp = "127.0.0.1";
Connection* TCPMsg::connection = nullptr;
QString Connection::hostIp = "solivision.pl";

Connection::Connection(QWidget *parent) :
    QWidget(parent),
    ui(new Ui::Connection)
{
    ui->setupUi(this);
    QFile file("config.json");
    file.open(QFile::ReadOnly);
    if(file.isOpen()) {
        auto data = file.readAll();
        if(data.length()>5) {
            auto jsonobj = QJsonDocument::fromJson(data).object();
            Connection::hostIp = jsonobj.value("server").toString("solivision.pl");
        }
    }
    file.close();
    reconnectTimer = new QTimer();
    reconnectTimer->setInterval(4000);
    connect(reconnectTimer, &QTimer::timeout, this, &Connection::tryReconnect);
    reconnectTimer->start();
    tryReconnect();
    TCPMsg::connection = this;

}

void Connection::sendData(const QString& json) {
    if(!socket) return;
    if(!socket->isOpen()) return;
    qDebug()<<(json+'\n').toUtf8();
    socket->write((json+'\n').toUtf8());
}

Connection::~Connection()
{
    if(socket){
        socket->blockSignals(true);
        reconnectTimer->stop();
        socket->close();
    }
    delete socket;
    delete ui;
}

void Connection::readyRead() {
    auto raw = socket->readLine();

    auto jsonobj = QJsonDocument::fromJson(raw).object();
    auto response = jsonobj.value("action").toString();
    auto data = jsonobj.value("data").toObject();
    TCPMsg::tryInvoke(response, data);
    qDebug()<<data;
}

void Connection::tryReconnect()
{
    if(socket!=nullptr)
        if(socket->isOpen())
            socket->close();

    socket = new QTcpSocket();
    socket->connectToHost(hostIp, 1121);
    connect(socket, &QTcpSocket::connected, this, [this](){
        reconnectTimer->stop();
        ui->checkBox->setChecked(true);
        emit connected();
    });
    connect(socket, &QTcpSocket::disconnected, this, [this](){
        reconnectTimer->start();
        ui->checkBox->setChecked(false);
        emit disconnected();
    });
    connect(socket, &QTcpSocket::readyRead, this, &Connection::readyRead);
}

