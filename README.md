# BureaucraticOrganization
[TOC]
## Использование
### Классы
#### RuleEvent
Каждый отдел выполняет 3 действия. Класс `RuleEvent` определяет какую печать поставить, какую зачеркнуть и в какой отдел отправить Васю.

Следующий код создаст событие, которе поставит печать **C**, зачеркнёт печать **A** и отправит Васю в отдел **PhpStorm**:
```csharp
var event = new RuleEvent("C","A","PhpStorm");
```
#### Department
За представление отдела органзации отвечает класс `Department`.
Создание отдела **PyCharm** с безусловным правилом:
```csharp
var dep1 = new Department("PyCharm", new RuleEvent("C","A","PhpStorm"));
```
Создание отдела **PhpStorm** с условным правилом в зависимости от наличия печати **D**:
```csharp
var dep2 = new Department("PhpStorm","D", new RuleEvent("G","A","DataGrid"), new RuleEvent("A","B","Rider"))
```
#### OrganizationConfiguration
Хранит конфигурацию организации, а именно:
- Отдел с которого Вася начнёт обход
- Отдел на котором Вася закончит обход
- Список всех отедлов

Пример создания конфигурации:

**Шаг 1.** 
Создаём коллекцию отедлов
```csharp
List<Department> departments = new List<Department>()
{
	new Department("CLion", new RuleEvent("A","B","PyCharm")),
	new Department("PyCharm", new RuleEvent("C","A","PhpStorm")),
	new Department("PhpStorm","D", new RuleEvent("G","A","DataGrid"), new RuleEvent("A","B","Rider")),
	new Department("Rider", new RuleEvent("D","B","PhpStorm")),
	new Department("DataGrid", new RuleEvent("K","B","PhpStorm"))
};
```
**Шаг 2.**
Устанавливаем **CLion** стартовым отдлом, а **DataGrid** конечным.
```csharp
OrganizationConfiguration configuration = new OrganizationConfiguration("CLion","DataGrid",departments);
```


