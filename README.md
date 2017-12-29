# NetCoreDockerImage3Example #
NetCore c# 練習使用 SqlClient

# docker 編譯與執行說明 #
## docker 編譯
<pre>docker build -t dotnetmyapp4 .</pre>

執行成功會有以下訊息 (20ae9d152e3d 這個每次編譯都不一樣得值)
<pre>
Successfully built 20ae9d152e3d
Successfully tagged dotnetmyapp4:latest
</pre>

## docker 執行
<pre>
docker run dotnetmyapp4 -s 'server={localdb};database={database};uid={uid};pwd={pwd}'
</pre>
參數說明: 
-s : 設定db連線字串 (裡面 {XX} 中的XX請依照實際環境填入值)

執行成功將會看到下面訊息
<pre>
Init Sets:
-s
server={localdb};database={database};uid={uid};pwd={pwd}
Run myLibrary.Class1.TestRun.
DBConnectStr='server={localdb};database={database};uid={uid};pwd={pwd}'
RuleID:1.
RuleID:2.
RuleID:3.
RuleID:4.
RuleID:5.
RuleID:6.
RuleID:7.
RuleID:8.
RuleID:9.
RuleID:10.
RuleID:11.
</pre>
