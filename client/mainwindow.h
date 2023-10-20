#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <CarsDialog.h>
#include <QMainWindow>
#include <QProcess>
#include <QTimer>
#include <QtNetwork/QTcpSocket>

QT_BEGIN_NAMESPACE
namespace Ui { class MainWindow; }
QT_END_NAMESPACE

class MainWindow : public QMainWindow
{
    Q_OBJECT
    QProcess* runningInstance{};
    const static QString sapath;
    const static QString sevenZipPath;
    const static QString sevenZipURL;

public:
    MainWindow(QWidget *parent = nullptr);
    ~MainWindow();

private:
    Ui::MainWindow *ui;
private slots:
    void connectedToServer();
    void disconnectedFromServer();
    void tryPlay(const QString& login);
};
#endif // MAINWINDOW_H
