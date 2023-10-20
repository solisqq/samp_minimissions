#ifndef CONNECTION_H
#define CONNECTION_H

#include <QWidget>
#include <QTcpSocket>
#include <QJsonObject>
#include <QJsonDocument>

namespace Ui {
class Connection;
}

class Connection : public QWidget
{
    Q_OBJECT
    QTcpSocket* socket{};
    QTimer* reconnectTimer{};
    QByteArray dataBuffor = QByteArray();
public:
    static QString hostIp;
    explicit Connection(QWidget *parent = nullptr);
    ~Connection();
public slots:
    void sendData(const QString &);

private slots:
    void tryReconnect();
    void readyRead();
signals:
    void connected();
    void disconnected();
private:
    Ui::Connection *ui;
};

class TCPMsg : public QObject {
    Q_OBJECT
    static std::unordered_map<QString, TCPMsg*> messages;
    static Connection* connection;
    QString cmd;
public:
    TCPMsg(QString cmd): cmd(cmd) {
        messages.insert({cmd, this});
    }
    //We will try to make duplicate of given msg cmd in unordered map (which is not allowed)
    TCPMsg(const TCPMsg&) = delete;
    TCPMsg& operator=(const TCPMsg&) = delete;
    ~TCPMsg() {
        messages.erase(cmd);
    }
    static bool tryInvoke(const QString& cmd, const QJsonObject& data) {
        auto it = messages.find(cmd);
        if(it!=messages.end()) {
            emit it->second->responseReady(data);
            return true;
        }
        return false;
    }
    void send(QJsonObject &data) {
        auto doc = QJsonDocument(data);
        if(TCPMsg::connection){
            QString jsonString = "{ \"action\": \""+cmd+"\", \"data\": "+doc.toJson(QJsonDocument::Compact)+"}";
            TCPMsg::connection->sendData(jsonString);
        }
    }
signals:
    void responseReady(const QJsonObject& response);
    friend Connection;
};

#endif // CONNECTION_H
