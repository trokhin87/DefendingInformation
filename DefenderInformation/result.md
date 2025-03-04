# Отчет по реализации RSA-шифрования

## Описание проекта

Данный проект представляет собой реализацию алгоритма RSA для шифрования и дешифрования сообщений. Алгоритм основан на простых числах, их произведении и использовании модульной арифметики для создания ключей шифрования и дешифрования.

## Основные компоненты

### 1. **Класс `RSA`**
Этот класс реализует основные операции RSA-шифрования:
- Генерацию открытого и закрытого ключей.
- Шифрование строки.
- Дешифрование зашифрованных данных.

### 2. **Класс `Program`**
Точка входа в программу. Демонстрирует использование класса `RSA`, создавая экземпляр с фиксированными значениями простых чисел `p` и `q`, затем шифруя и расшифровывая строку `hello`.

---

## Детальный разбор кода

### **Инициализация RSA**
При создании объекта `RSA` выполняются следующие шаги:
1. Выбор двух простых чисел `p` и `q`.
2. Вычисление `n = p * q` — это модуль для шифрования и расшифрования.
3. Вычисление `phi = (p - 1) * (q - 1)` — значение функции Эйлера.
4. Установка стандартного значения `e = 65537`, которое широко используется в криптографии RSA.
5. Вычисление `d` — мультипликативного обратного к `e` по модулю `phi`, используя `ModInverse()`.

```csharp
n = p * q;
phi = (p - 1) * (q - 1);
e = 65537;
d = ModInverse(e, phi);
```

### **Метод `ModInverse`**
Этот метод вычисляет мультипликативное обратное `d` к `e` по модулю `phi` с использованием расширенного алгоритма Евклида.

```csharp
private static BigInteger ModInverse(BigInteger a, BigInteger m) {
    BigInteger m0 = m, t, q;
    BigInteger x0 = 0, x1 = 1;
    while (a > 1) {
        q = a / m;
        t = m;
        m = a % m;
        a = t;
        t = x0;
        x0 = x1 - q * x0;
        x1 = t;
    }
    return x1 < 0 ? x1 + m0 : x1;
}
```

### **Шифрование (`Encrypt`)**
Метод преобразует строку в массив байтов ASCII и затем шифрует каждый байт с использованием `BigInteger.ModPow()`.

```csharp
public BigInteger[] Encrypt(string message) {
    byte[] bytes = Encoding.ASCII.GetBytes(message);
    BigInteger[] encrypted = new BigInteger[bytes.Length];
    for (int i = 0; i < bytes.Length; i++) {
        encrypted[i] = BigInteger.ModPow(bytes[i], e, n);
    }
    return encrypted;
}
```

### **Дешифрование (`Decrypt`)**
Метод принимает массив зашифрованных чисел, применяет `BigInteger.ModPow()` с закрытым ключом `d` и преобразует результат обратно в строку.

```csharp
public string Decrypt(BigInteger[] encrypted) {
    byte[] bytes = new byte[encrypted.Length];
    for (int i = 0; i < encrypted.Length; i++) {
        BigInteger decrypted = BigInteger.ModPow(encrypted[i], d, n);
        if (decrypted < 0 || decrypted > 255) {
            throw new Exception("Ошибка: декодированный символ выходит за пределы ASCII!");
        }
        bytes[i] = (byte)decrypted;
    }
    return Encoding.ASCII.GetString(bytes);
}
```

### **Тестирование в `Main`**
Программа создает объект `RSA`, шифрует сообщение `hello`, а затем его расшифровывает.

```csharp
static void Main(string[] args) {
    RSA rsa = new RSA(61, 53);
    string message = "hello";
    BigInteger[] encrypt = rsa.Encrypt(message);
    foreach (BigInteger b in encrypt) {
        Console.Write(b+" ");
    }
    Console.WriteLine();
    string message2 = rsa.Decrypt(encrypt);
    Console.WriteLine(message2);
}
```

---

## Выводы
1. Реализована базовая версия RSA-шифрования с фиксированными простыми числами `p` и `q`.
2. Код демонстрирует основные принципы работы RSA: выбор ключей, шифрование и дешифрование.
3. Шифрование работает на уровне отдельных символов ASCII, что делает его применимым для небольших сообщений.
4. В реальных системах необходимо использовать более крупные простые числа (`p` и `q`), а также дополнительную защиту данных.

