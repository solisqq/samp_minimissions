#include "mainwindow.h"
#include "ui_mainwindow.h"
#include "static/StylesheetLoader.h"


/*.\f -c -h 127.0.0.1 -p 7777 -n Solis*/
/*https://www.7-zip.org/a/7z2301-extra.7z*/

//const QString MainWindow::hostIp = "127.0.0.1";
const QString MainWindow::sapath = "G:/Games/SAMP/Game/GTA San Andreas";
const QString MainWindow::sevenZipPath = "3rdparty/7zr.exe";
const QString MainWindow::sevenZipURL = "https://www.7-zip.org/a/7zr.exe";

MainWindow::MainWindow(QWidget *parent)
    : QMainWindow(parent)
    , ui(new Ui::MainWindow)
{
    ui->setupUi(this);
    StylesheetLoader::loadInto("main.css", this);
    ui->loginWidget333->hide();
    ui->playerWidget->hide();
    ui->playerCarWidget->hide();
    connect(ui->playerWidget, &PlayerData::runRequested, this, &MainWindow::tryPlay);
    connect(ui->connectionWidget, &Connection::connected, this, &MainWindow::connectedToServer);
    connect(ui->connectionWidget, &Connection::disconnected, this, &MainWindow::disconnectedFromServer);
    connect(ui->loginWidget, &Session::sessionAuthorized, ui->playerWidget, [this](const QString& login){
        ui->loginWidget->hide();
        ui->playerWidget->updateData(login);
        ui->playerWidget->show();
        ui->playerCarWidget->show();
        ui->playerCarWidget->logedIn(login);
    });
    connect(ui->playerCarWidget, &PlayerCarWidget::refreshUserData, ui->playerWidget, &PlayerData::updateData);
}

MainWindow::~MainWindow()
{
    if(runningInstance) {
        runningInstance->kill();
        runningInstance->close();
        runningInstance->deleteLater();
    }
    delete ui;
}

void MainWindow::connectedToServer()
{
    ui->loginWidget333->show();
}

void MainWindow::disconnectedFromServer()
{
    ui->loginWidget333->hide();
    ui->playerWidget->logout();
    ui->loginWidget->logout();
    ui->loginWidget->show();
    ui->playerWidget->hide();
    ui->playerCarWidget->hide();
}

void MainWindow::tryPlay(const QString& login)
{
    if(login.size()<4) return;
    if(runningInstance) {
        runningInstance->kill();
        runningInstance->close();
        runningInstance->deleteLater();
    }
    runningInstance = new QProcess();
    connect(runningInstance, &QProcess::readyReadStandardOutput, this, [this](){
        qDebug()<<this->runningInstance->readAllStandardOutput();
    });
    connect(runningInstance, &QProcess::readyReadStandardError, this, [this](){
        qDebug()<<this->runningInstance->errorString();
    });
    QString fullcmd = "sampcmd.exe -c -h "+Connection::hostIp+" -p 7777 -n "+login;
    runningInstance->startCommand(fullcmd);
    qDebug()<<fullcmd;
}



