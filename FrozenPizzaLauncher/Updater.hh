#ifndef UPDATER_HH_
#define UPDATER_HH_

#include <QApplication>
#include <QtGui>
#include <QtNetwork/QNetworkAccessManager>
#include <QtNetwork/QNetworkRequest>
#include <QtNetwork/QNetworkReply>
#include <QCoreApplication>
#include <QUrl>

class Updater : public QWidget
{
  Q_OBJECT

public:
  Updater();

 

protected:

  QPushButton *updateBtn;

  QNetworkReply *netReply;

  QString currentVersion; // Version actuelle

  QString newVersion; // Version de la nouvelle version si elle existe

  QLabel label;

 

public slots:

  void startUpdate(); //Slot de mise à jour

};

#endif // TUTO_H
