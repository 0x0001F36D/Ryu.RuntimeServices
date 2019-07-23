### Overview

對 .Net Framework 的基本功能延伸及擴充

### Namespaces

- RuntimeServices.Callable 
  > 提供執行期中對建構物件的中繼層支援，主要類別為 Contraint\<T> 類別，使用委派進行物件的建構及鬆散耦合，可使用 精確模式(參數類型必須完全一致) 
  > 及 模糊匹配模式(參數可於搜尋後再給或直接輸出所有的物件) 作為執行期建構函式

- RuntimeServices.DLR
  > 提供物件對 ```dynamic``` 的自定轉換，主要類別為 Synthesis 類別

- Diagnostics
  > 提供物件自我診斷之能力

- Windows.Shell
  > 提供對 Windows 平台下的的桌面及工具列支援，實作 TaskbarProgressBar 類別及對 Desktop 對桌面圖示的控制支援
