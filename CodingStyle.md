The coding style used in this project will be similar to [conventions suggested by Microsoft](https://msdn.microsoft.com/en-us/library/ff926074.aspx
).

So use the format feature in Visual Studio / Mono as much as possible!!

---

We will also follow some of our own conventions:

* Try to use `var` keyword as much as possible.

* `private`, `readonly`, etc keywords should be explicit.

* Multiple statements per line are fine and expected.
	```c#
	// if they are similar
	x = Input.GetAxis("Horizontal"); y = Input.GetAxis("Vertical");
  ```
	
	```c#
  // if they are in declaration
  bool someBool, aBool = true;
  ```
  
* Use of null coalescing (??) is highly appreciatated.
  ```c#
  // use this
  var set = localDefault ?? globalDefault;

  // instead of
  Setting set;
  if(localdefault != null)
    set = localDefault;
  else
    set = globalDefault;
  ```

* Use of ternary operator (?:) is highly appreciatated.
  ```c#
  // use this
  var speed = Input.GetButton("Walk") ? 5 : 10;  
  
  // instead of
  float speed;
  if(Input.GetButton("Walk"))
    speed = 5;
  else
    speed = 10;
  ```

* Use of properties is highly appreciatated.
	
* Try to avoid braces if only one statement is required.
  And start the statement from new line.
  ```c#
  // use this
  if(ready)
    What();
  foreach(var char in characters)
    char.Move();
 
  // instead of
  if(ready)
  {
    What();
  }
  ```		

* Newlines are expected(as less as possible) between:
  
  * Logical sections in functions
  * Between functions
  * Between fields which are of different use
