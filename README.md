###


### 己完成
admin可以用reset重設一般使用者的密碼
新增商品賣家
前端介面先整理一下
處理checkout 哇塞~要不斷用資料表的關聯性去對照，真的有點複雜，卡爆	解決了 關聯性拉起來，ef物件就會自己加入指向的物件，方便很多，只可惜如果linq拉出來到list然後用foreach迭代方法，裡面物件會是空的，還是要自己再去抓
新增商品己結帳
新增商品物流進度 > 可由賣家改物流進度了
加入雙語言功能，好好玩，做到一半，算了，本來想做動態的，但是得用Session就得用db去存各個使用者偏好的


### 當前進度

直接搞聊天室了啦~等套件 要用asp.net認證 看gpt

買賣家審認系統

結單功能: 賣家可以將訂單改成




### 未來進度

把驗證方式改成用annotation(ActionFilter)	>先暫緩，對目前好像效益沒到很大
:::info:::
這邊先不要，等物流做完，因為我突然想到，如果賣家刪了，結果還有人東西沒到貨怎麼辦
MyProduct頁面刪除商品 
Products.DeleteProduct呼叫
ProductData執行圖片刪除及從資料表中移除
:::info:::

新增與賣家聊天功能(哇鳴~)

#### Session格式

* Id 最終還是妥協了
* Username = 就db的Username欄位
* Nickname
* Role
	0 = Administrator
	1 = Coleader
	2 = User

### 已知有，但還沒修的bug

1. signup頁面如果沒輸入會跳錯誤



### 筆記區

#### 語言功能

:::info
需要在專案中加入
/Resources/Lang.resx
/Resources/Lang.zh-TW.resx

然後在web.config加入(這是為了不讓瀏覽器的語言header決定)
```
  <system.web>
	  <globalization
		  culture="en-US"
		  uiCulture="en-US"
		  enableClientBasedCulture="false"/>
  </system.web>
```
在程式內加入
```csharp
Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
Thread.CurrentThread.CurrentUICulture= new CultureInfo("en-US");
```
前端用
```
Lang.[Keys]
```
就可以了
* 記得 resx要建
如果想要實現隨使用者設定值，就得在global.asax中的`Application_BeginRequest`設culture
:::