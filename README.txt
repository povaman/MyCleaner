خطوات التشغيل (مرة واحدة):
1. ثبّت .NET 8 SDK من: https://dotnet.microsoft.com/download/dotnet/8.0
2. ثبّت Inno Setup 6 من: https://jrsoftware.org/isdl.php
3. افتح PowerShell جوه فولدر MyCleaner واكتب: .\build.ps1
4. هيطلع ملف Setup في فولدر output، وهيطبع الـ SHA256.
5. ارفع الكود على GitHub (repo اسمه MyCleaner)، واعمل Release جديد بتاج v1.0.0 وارفع ملف Setup فيه.
6. حط الـ SHA256 في version.json وارفعه على الـ repo.

لإصدار جديد: غيّر الرقم في MyCleaner.csproj و setup.iss و version.json، شغّل build.ps1، وارفع Release جديد.
