#ifndef SKINDIALOG_H
#define SKINDIALOG_H

#include <QDialog>

namespace Ui {
class SkinDialog;
}

class SkinDialog : public QDialog
{
    Q_OBJECT
public:
    explicit SkinDialog(QWidget *parent = nullptr);
    ~SkinDialog();

private:
    Ui::SkinDialog *ui;
signals:
    void skinSelected(int id);
};

#endif // SKINDIALOG_H
