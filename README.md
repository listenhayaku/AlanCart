###


### 己完成
admin可以用reset重設一般使用者的密碼
新增商品賣家
前端介面先整理一下
處理checkout 哇塞~要不斷用資料表的關聯性去對照，真的有點複雜，卡爆	解決了 關聯性拉起來，ef物件就會自己加入指向的物件，方便很多，只可惜如果linq拉出來到list然後用foreach迭代方法，裡面物件會是空的，還是要自己再去抓
新增商品己結帳
新增商品物流進度 > 可由賣家改物流進度了

### 當前進度



把驗證方式改成用annotation(ActionFilter)	>先暫緩，對目前好像效益沒到很大



### 未來進度


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