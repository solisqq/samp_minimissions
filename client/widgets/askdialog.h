#ifndef ASKDIALOG_H
#define ASKDIALOG_H

#include <QDialog>

namespace Ui {
class AskDialog;
}

class AskDialog : public QDialog
{
    Q_OBJECT

public:
    explicit AskDialog(QWidget *parent = nullptr);
    ~AskDialog();

private:
    Ui::AskDialog *ui;
};

#endif // ASKDIALOG_H
