###


### 己完成
admin可以用reset重設一般使用者的密碼
新增商品賣家

### 當前進度
前端介面先整理一下

新增商品己結帳
新增商品物流進度


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