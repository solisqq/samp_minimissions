#ifndef SESSION_H
#define SESSION_H

#include <QWidget>
#include <connection.h>
#include <QFile>

namespace Ui {
class Session;
}

class Session : public QWidget
{
    Q_OBJECT
    TCPMsg loginMsg;
    QFile file = QFile("config.json");
    QString password="";
public:
    explicit Session(QWidget *parent = nullptr);
    ~Session();

public slots:
    void logout();
private:
    Ui::Session *ui;
private slots:
    void handleLogin();
signals:
    void sessionAuthorized(const QString& login);
};

#endif // SESSION_H
