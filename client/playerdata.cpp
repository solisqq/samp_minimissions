#include "playerdata.h"
#include "ui_playerdata.h"

#include <QTimer>

PlayerData::PlayerData(QWidget *parent) :
    QWidget(parent),
    playerInfoMsg("player_info"),
    requestSkinChangeMsg("change_skin"),
    ui(new Ui::PlayerData)
{
    ui->setupUi(this);
    connect(&playerInfoMsg, &TCPMsg::responseReady, this, [this](const QJsonObject& obj){
        auto response = obj.value("status").toString("fail");
        if(response!="success") return;
        QString path = ":/skins/normal/Skin_"+obj.value("skin").toString("1")+".png";
        qDebug()<<path;
        ui->skinBtn->setStyleSheet("border-image:url("+path+");");
        ui->loginLabel->setText(obj.value("login").toString("User"));
        ui->scoreLabel->setText(obj.value("score").toString("0"));
    });
    updateTimer = new QTimer(this);
    updateTimer->setInterval(20000);
    connect(updateTimer, &QTimer::timeout, this, [this](){
        updateData(prevLogin);
    });
    connect(ui->pushButton, &QPushButton::clicked, this, [this](){
        emit runRequested(prevLogin);});
    skinDialog = new SkinDialog(this);
    connect(ui->skinBtn, &QPushButton::clicked, skinDialog, &SkinDialog::show);
    connect(skinDialog, &SkinDialog::skinSelected, this, [this](int id){
        QJsonObject obj;
        obj.insert("login", prevLogin);
        obj.insert("skin", QString::number(id));
        requestSkinChangeMsg.send(obj);
        ui->skinBtn->setStyleSheet("border-image:url(:/skins/normal/Skin_"+QString::number(id)+".png"+");");
    });
}

void PlayerData::updateData(const QString& login) {
    QJsonObject obj;
    obj.insert("login", login);
    prevLogin = login;
    playerInfoMsg.send(obj);
    updateTimer->start();
}

void PlayerData::logout() {
    prevLogin="";
    updateTimer->stop();
}

void PlayerData::setVisible(bool visible)
{
    return QWidget::setVisible(visible);
}

PlayerData::~PlayerData()
{
    delete ui;
}
