#include "session.h"
#include "ui_session.h"
#include <QDesktopServices>
#include <connection.h>
#include <QMessageBox>

Session::Session(QWidget *parent) :
    QWidget(parent),
    loginMsg("auth"),
    ui(new Ui::Session)
{
    ui->setupUi(this);
    connect(ui->pushButton, &QPushButton::clicked, this, &Session::handleLogin);
    connect(&loginMsg, &TCPMsg::responseReady, this, [this](const QJsonObject& obj){
        auto jsonVal = obj.value("status");
        auto response = jsonVal.toString("fail");
        if(response=="success") {
            emit sessionAuthorized(obj.value("login").toString("User"));
        }
        else QMessageBox::warning(this, "Niepoprawne dane", "Twoje hasło bądź login są niepoprawne.");
    });
    connect(ui->createLinkBtn, &QPushButton::clicked, this, [](){
        QDesktopServices::openUrl(QUrl("http://www.solivision.pl/wzg/"));
    });
    //TO REMOVE
    file.open(QFile::ReadWrite);
    if(file.isOpen()) {
        auto data = file.readAll();
        if(data.length()>5) {
            auto jsonobj = QJsonDocument::fromJson(data).object();
            ui->loginEdit->setText(jsonobj.value("login").toString(""));
            ui->passEdit->setText(jsonobj.value("password").toString(""));
        }
    }
    file.close();
}

Session::~Session()
{
    delete ui;
}

void Session::logout() {
//    ui->passEdit->setText("");
}

void Session::handleLogin()
{
    QString login = ui->loginEdit->text();
    QString password = ui->passEdit->text();
    if(login.length()<4 || password.length()<6){
        QMessageBox::warning(this, "Niepoprawne logowanie", "Wpisałeś zbyt krótki login lub hasło.");
        return;
    };
    QJsonObject obj;
    obj.insert("login", login);
    obj.insert("password", password);
    loginMsg.send(obj);
    file.open(QFile::WriteOnly);
    if(file.isOpen()) {
        QJsonObject obj;
        obj.insert("login", login);
        if(Connection::hostIp!="solivision.pl")
            obj.insert("server", Connection::hostIp);
        if(password!="")
            obj.insert("password", password);
        auto doc = QJsonDocument(obj);
        file.write(doc.toJson(QJsonDocument::Compact));
    }
    file.close();
}
