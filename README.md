# BureaucraticOrganization
# Инициализация
### RuleEvent
Каждый отдел выполняет 3 действия. Класс `RuleEvent` определяет какую печать поставить, какую зачеркнуть и в какой отдел отправить Васю.

Следующий код создаст событие, которе поставит печать **C**, зачеркнёт печать **A** и отправит Васю в отдел **PhpStorm**:
```csharp
var event = new RuleEvent("C","A","PhpStorm");
```
### Department
За представление отдела органзации отвечает класс `Department`.
Создание отдела **PyCharm** с безусловным правилом:
```csharp
var dep1 = new Department("PyCharm", new RuleEvent("C","A","PhpStorm"));
```
Создание отдела **PhpStorm** с условным правилом в зависимости от наличия печати **D**:
```csharp
var dep2 = new Department("PhpStorm","D", new RuleEvent("G","A","DataGrid"), new RuleEvent("A","B","Rider"))
```
### OrganizationConfiguration
Хранит конфигурацию организации, а именно:
- Отдел с которого Вася начнёт обход
- Отдел на котором Вася закончит обход
- Список всех отедлов

Пример создания конфигурации:

**Шаг 1**
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
**Шаг 2**
Устанавливаем **CLion** стартовым отделом, а **DataGrid** конечным.
```csharp
var configuration = new OrganizationConfiguration("CLion","DataGrid",departments);
```
### Organization
Все предыдущие классы нужны были создания конфигурации организации. Класс `Organization` служит для получения ответов на запрос поставленный в задаче.

#### Инициализаия через код
```csharp
Organization organization = new Organization(configuration);
```
#### Инициализаия через JSON
Создадим example.json по следующему шаблону:
```json
{
  "departments": [
    {
      "rule": {
        "event": {
          "putStampId": "A",
          "crossStampId": "B",
          "nextDepartmentId": "PyCharm"
        }
      },
      "id": "CLion"
    },
    {
      "rule": {
        "event": {
          "putStampId": "C",
          "crossStampId": "A",
          "nextDepartmentId": "PhpStorm"
        }
      },
      "id": "PyCharm"
    },
    {
      "rule": {
        "conditionalStamp": "D",
        "event1": {
          "putStampId": "A",
          "crossStampId": "B",
          "nextDepartmentId": "DataGrid"
        },
        "event2": {
          "putStampId": "A",
          "crossStampId": "B",
          "nextDepartmentId": "Rider"
        }
      },
      "id": "PhpStorm"
    },
    {
      "rule": {
        "event": {
          "putStampId": "D",
          "crossStampId": "B",
          "nextDepartmentId": "PhpStorm"
        }
      },
      "id": "Rider"
    },
    {
      "rule": {
        "event": {
          "putStampId": "D",
          "crossStampId": "B",
          "nextDepartmentId": "PhpStorm"
        }
      },
      "id": "DataGrid"
    }
  ],
  "startDepartment": "CLion",
  "endDepartment": "PhpStorm"
}
```
Создадим организацию:
```csharp
Organization organization;
using (StreamReader file = File.OpenText(yourpath = "example.json")
{
	organization = new Organization(file.ReadToEnd());
}
```
Также можно сменить конфигурацию вызвав метод  Configure() у organization
```csharp
organization.Configure(anotherConfiguration);
organization.Configure(jsonConfiguration);
```
# Использование
После задания конфигурации, можно послать запрос вида **"какие незачеркнутые печати есть в обходном листе Васи, когда он в ходе путешествия выходит из отдела Q"**. Для избежания зацикливания, можно задать максимальное время вычисления (по умолчанию - 1 секунда).
```csharp
BypassResult result = await organization.GetResultAsync("CLion");
//C заданием максимального времени вычисления
BypassResult result = await organization.GetResultAsync("CLion"),TimeSpan.FromMilliseconds(500));
if(result.Successful)
{
var sheetSnapshots = result.BypassSheetSnapshots;
	foreach(var snapshot in sheetSnapshots)
	{
		foreach(var stamp in snapshot.Stamps)
		{
			Console.WriteLine($"Печать {stamp.Id} - {stamp.State}");
		}
		Console.WriteLine();
	}
}
else
{
	Console.WriteLine(result.Exception.Message);
}
//Или в формате JSON
Console.WriteLine(result.ToJson());
```
Пример ответа при успешном запросе:
```json
{
  "successful": "true",
  "snapshots": [
    {
      "stamps": [
        {
          "id": "A",
          "state": "Putted"
        },
        {
          "id": "C",
          "state": "Putted"
        }
      ]
    },
    {
      "stamps": [
        {
          "id": "A",
          "state": "Crossed"
        },
        {
          "id": "C",
          "state": "Putted"
        },
        {
          "id": "D",
          "state": "Putted"
        },
        {
          "id": "G",
          "state": "Putted"
        }
      ]
    }
  ]
}
```
Пример ответа при неудаче:
```json
{
  "successful": "false",
  "exception": "Calculation time exceeded"
}
```




