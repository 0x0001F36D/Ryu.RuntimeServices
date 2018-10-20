### Overview

[![Build status](https://ci.appveyor.com/api/projects/status/q61qn3928e9y8akx?svg=true)](https://ci.appveyor.com/project/0x0001F36D/Ryuko-runtimeservices)

提供對執行期的物件進行轉換以及擴充

### Namespaces
  
  - #### Callable 
    提供執行期中對建構物件的中繼層支援，主要類別為 Contraint\<T> 類別，使用委派進行物件的建構及鬆散耦合，可使用 精確模式(參數類型必須完全一致) 
及 模糊匹配模式(參數可於搜尋後再給或直接輸出所有的物件) 作為執行期建構函式

  - #### DLR
    提供物件對 dynamic 的自定轉換，主要類別為 Synthesis 類別
    


