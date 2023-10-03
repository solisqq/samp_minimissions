#ifndef PLAYERDATA_H
#define PLAYERDATA_H

#include "connection.h"
#include <QWidget>
#include <SkinDialog.h>

namespace Ui {
class PlayerData;
}

class PlayerData : public QWidget
{
    Q_OBJECT
    QTimer *updateTimer{};
    TCPMsg playerInfoMsg;
    TCPMsg requestSkinChangeMsg;
    QString prevLogin="";
    SkinDialog* skinDialog;
public:
    explicit PlayerData(QWidget *parent = nullptr);
    ~PlayerData();
public slots:
    void updateData(const QString& login);

signals:
    void runRequested(const QString& login);

private:
    Ui::PlayerData *ui;

    // QWidget interface
public slots:
    void setVisible(bool visible) override;
    void logout();
};

#endif // PLAYERDATA_H
