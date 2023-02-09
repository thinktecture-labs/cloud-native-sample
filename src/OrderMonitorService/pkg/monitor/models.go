package monitor

type OrderListModel struct {
	Id        string              `json:"id"`
	UserId    string              `json:"userId"`
	Positions []PositionListModel `json:"positions"`
}

type PositionListModel struct {
	ProductId          string  `json:"productId"`
	ProductName        string  `json:"productName"`
	ProductPrice       float64 `json:"productPrice"`
	ProductDescription string  `json:"productDescription"`
	Quantity           int     `json:"quantity"`
}
