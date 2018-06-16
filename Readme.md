### *Constraint&lt;T&gt;*

- ## 用途
    高效能的動態物件實體產生器 (*emit* + *delegate cache*)
    稍微修改能做為 *void .ctor(...)* 的 *hooker*
- ## 原理
    
    這是一個類別 *(class)* 與其之建構子 *(.ctor)*
    ``` csharp  
    public class Test // class
    {
        public Test() //.ctor
        {
        }
    }
    ```
    這是一個委派 *(delegate)* 且委派的參數與上面的建構子相同<br>
    回傳是上面的 *Test* 類型
    *P.s: 這個委派也是動態製作的，我用 TypeBuilder 類別製作動態的委派類型*
    ``` csharp
    public delegate Test TestConstructor();
    ```
    
    我用 *Emit* 的方式動態製作上述的委派實體方法，其實現的程式碼就像下面這樣
    ``` csharp 
    var testCtor = new TestConstructor(()=> new Test());
    ```
    
- ## 用法
  
    $<$?$>$ 必須是 介面、抽象類別或者類別 等可被實作或繼承的結構
    ```csharp
    var collector = Constraint<?>.Collector;
    ```
    <br>

    #### Strict 模式
    \<??\> 必須是實作或繼承自 \<?\> 介面或類別的對象或子類別<br>
    會透過 \<??\> 及參數鎖定回傳的類型<br>
    ***注意 : New() 及其多載函數是一個強型別函數，若建構子引數型態為 object ，請將你的參數轉型成 object 型態***
    ```csharp
    var strict = collector.Strict<??>();
    var obj = strict.New(...); 
    ```
    
    <br>
    
    #### Fuzzy 模式
    透過參數群推斷建構子引數相同之類型並透過 *NewAll()* 函數建立多個物件實體
    ```csharp
    var fuzzy = collector.Fuzzy(...);
    var objs = fuzzy.NewAll();
    ```
    <br>

- ## 歷史
  - v2.2: <br>
    整理程式碼，新增 *Types* 類別，加入nuget包
  - v2.1: <br>
    整理程式碼，實作 *Fuzzy* 模式，可透過參數模糊匹配物件
  - v2: <br>
    將 *CallSite\<T>* 類別移除，改用 *Constraint\<TConstraint>* 類別作為主類別<br>
    並優化類別的快取查詢功能以及新增 *Readme.md*
  - v1: <br>
    使用 *CallSite\<T>* 類別做為基底類別 


    