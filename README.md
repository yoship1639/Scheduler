# Scheduler
C#で時間操作を簡単に行うためのライブラリ

指定時間後に処理を発火させたり、指定間隔ごとに処理を発火させる処理を簡潔に記述できる。
主にゲームプログラムにおいて時間処理を記述することが多いため、それらを手助けする。

## Overview

C#において時間の間隔処理はTimer.Elapsedのイベント登録を行い、Timer.Start()で開始させる。
また、指定時間後に処理を発火させるには、Taskを定義し、Thread.Sleepを行って停止させた後処理を記述しなければならない。

これらの煩雑な処理をSchedulerによって簡潔に記述できるようになる

時間間隔処理の入れ子も可能で、例えば「5秒ごとに「0.1秒ごとに10回Tickを出力する」する処理を実行」という処理も可能である。

## Example

・3秒後にHello world!を出力する

```
Schedule.Add(3.0, () => Console.WriteLine("Hello world!"));
```

・3秒後に1秒間隔で無限にHello world!を出力する

```
Schedule.Repeat(3.0, 1.0, -1, () => Console.WriteLine("Hello world!"));
```

・5秒ごとに「0.1秒ごとに10回Tickを出力する」する処理を実行

```
Schedule.Repeat(0.0, 5.0, -1, () =>
{
    Schedule.Repeat(0.0, 0.1, 10, () => Console.WriteLine());
});
```
