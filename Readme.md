## *Viyrex.RuntimeServices*

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
  
    \<?\> 必須是 介面、抽象類別或者類別 等可被實作或繼承的結構
    ```csharp
    // <?>: Abstract class or Class or Interface
    // Gets Collector for make dynamic object
    var collector = Constraint<?>.Collector;
    ```
    
    \<??\> 必須是實作或繼承自 \<?\> 介面或類別的對象或子類別<br>
    *Return* 泛型函數會回傳一個 *LockedReturn* 結構用於在執行期建構動態 \<??\> 物件<br>
    你可以透過結構內的 *New()* 泛型函數做為建構物件的代理建構子函數以在執行期建立物件
    ```csharp
    // <??>: implemented or inherited from <?>
    // Return<??>() function will return the LockedReturn type
    // for call the New() function
    // The New(...) function is polymorphism function for strong type
    // arguments to creating the <??> object in runtime
    var obj = collector.Return<??>().New<...>(...);
    ```
- ## 歷史
  - v2: <br>
    將 *CallSite\<T>* 類別移除，改用 *Constraint\<TConstraint>* 類別作為主類別<br>
    並優化類別的快取查詢功能以及新增 *Readme.md*
  - v1: <br>
    使用 *CallSite\<T>* 類別做為基底類別 
    