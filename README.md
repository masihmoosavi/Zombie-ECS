# Zombie ECS

یک پروژه آموزشی و عملی برای پیاده‌سازی یک بازی ساده با استفاده از **Unity ECS (Entity Component System)**.

در این پروژه، تعدادی زامبی از اطراف یک قبرستان ظاهر می‌شوند، از زیر زمین بیرون می‌آیند و به سمت یک مغز بزرگ در مرکز محیط حرکت می‌کنند. زمانی که زامبی به محدوده مغز برسد، شروع به خوردن آن می‌کند و به مرور از سلامت مغز کم می‌شود.

## Gameplay

هسته اصلی بازی شامل سه مرحله برای رفتار زامبی‌ها است:

1. **Rise**  
   زامبی از محل Spawn در زیر زمین ایجاد شده و به سمت سطح زمین حرکت می‌کند.

2. **Walk**  
   پس از رسیدن به سطح زمین، زامبی به سمت مغز حرکت می‌کند و هنگام راه رفتن حرکت جانبی (Sway) دارد.

3. **Eat**  
   زمانی که زامبی به محدوده مغز برسد، حرکت خود را متوقف کرده و شروع به خوردن مغز می‌کند. در این مرحله، به صورت مداوم به سلامت مغز آسیب وارد می‌شود.

با کاهش سلامت مغز، اندازه آن نیز متناسب با میزان سلامت باقی‌مانده کاهش پیدا می‌کند.

## Technologies

- Unity
- C#
- Unity Entities / ECS
- Burst Compiler
- Job System
- ISystem
- IJobEntity
- IAspect
- EntityCommandBuffer
- DynamicBuffer
- Enableable Components
- LocalTransform

## ECS Architecture

این پروژه از معماری **Entity Component System** استفاده می‌کند. داده‌های مربوط به موجودیت‌ها در Componentها نگهداری شده و منطق بازی توسط Systemها پردازش می‌شود.

### Authoring & Baking

برای تنظیم اطلاعات در Unity Editor از MonoBehaviourهای زیر استفاده شده است:

- `BrainMono`
- `GraveyardMono`
- `ZombieMono`

Bakerهای مربوط به این کلاس‌ها، اطلاعات Authoring را هنگام Baking به Entity و Componentهای ECS تبدیل می‌کنند.

### Brain

مغز بازی دارای اطلاعات مربوط به سلامت خود است:

- `BrainHealth`
- `BrainTag`
- `BrainDamageBufferElement`

آسیب‌های واردشده توسط زامبی‌ها در یک Dynamic Buffer ذخیره شده و سپس توسط `ApplyBrainDamageSystem` روی سلامت مغز اعمال می‌شوند.

### Graveyard

قبرستان مسئول مدیریت محیط و Spawn زامبی‌ها است.

اطلاعات اصلی آن در `GraveyardProperties` قرار دارد و شامل مواردی مانند:

- ابعاد محیط
- تعداد قبرسنگ‌ها
- Prefab قبرسنگ
- Prefab زامبی
- نرخ Spawn زامبی

موقعیت‌های Spawn زامبی‌ها نیز در `DynamicBuffer<ZombieSpawnPoint>` ذخیره می‌شوند.

### Zombie

رفتار زامبی‌ها به چند بخش مستقل تقسیم شده است:

- `ZombieRiseAspect` برای بیرون آمدن از زمین
- `ZombieWalkAspect` برای حرکت به سمت مغز
- `ZombieEatAspect` برای آسیب زدن به مغز

این تفکیک باعث می‌شود هر مرحله از رفتار زامبی به صورت مستقل توسط Componentها و Systemهای مربوط به خود مدیریت شود.

## Systems

سیستم‌های اصلی پروژه عبارت‌اند از:

### `SpawnTombstoneSystem`

در ابتدای بازی قبرسنگ‌ها را به صورت تصادفی در محیط ایجاد می‌کند و برای هر قبرسنگ یک نقطه Spawn زامبی ایجاد می‌کند.

### `SpawnZombieSystem`

بر اساس تایمر Spawn، زامبی جدید ایجاد کرده و آن را در یکی از نقاط Spawn قرار می‌دهد.

### `InitializeZombieSystem`

وضعیت اولیه زامبی‌های جدید را تنظیم می‌کند و Componentهای مربوط به حرکت و خوردن را غیرفعال می‌کند تا زامبی ابتدا وارد مرحله Rise شود.

### `ZombieRiseSystem`

زامبی را از زیر زمین به سمت سطح زمین حرکت می‌دهد. پس از رسیدن به سطح، Component مربوط به Rise حذف شده و حرکت زامبی فعال می‌شود.

### `ZombieWalkSystem`

زامبی را به سمت مغز حرکت می‌دهد. زمانی که زامبی به محدوده مشخصی از مغز برسد، حرکت متوقف شده و حالت Eat فعال می‌شود.

### `ZombieEatSystem`

زامبی را در حالت خوردن قرار می‌دهد و بر اساس مقدار `EatDamagePerSecond` به مغز آسیب وارد می‌کند.

### `ApplyBrainDamageSystem`

آسیب‌های ذخیره‌شده در `BrainDamageBufferElement` را روی سلامت مغز اعمال می‌کند و اندازه مغز را متناسب با سلامت باقی‌مانده تغییر می‌دهد.

### `CameraControllerSystem`

حرکت دوربین را بر اساس اندازه فعلی مغز کنترل می‌کند. با کوچک شدن مغز، فاصله و ارتفاع دوربین نیز تغییر می‌کند تا صحنه از زاویه مناسب نمایش داده شود.

## Performance

یکی از اهداف اصلی پروژه، استفاده از قابلیت‌های ECS برای پردازش تعداد زیادی Entity به شکل بهینه است.

برای این منظور از موارد زیر استفاده شده است:

- **Burst Compiler** برای اجرای سریع‌تر کدهای محاسباتی
- **Job System** برای پردازش موازی زامبی‌ها
- **IJobEntity** برای اجرای منطق روی Entityهای موردنظر
- **ScheduleParallel** برای اجرای موازی Jobها
- **EntityCommandBuffer** برای انجام تغییرات ساختاری به صورت امن و Deferred
- **Enableable Components** برای فعال و غیرفعال کردن رفتارهای زامبی بدون حذف Component

## Project Structure

ساختار کلی کد پروژه به صورت زیر سازمان‌دهی شده است:

```text
Zombie-ECS
├── Authoring
│   ├── BrainMono
│   ├── GraveyardMono
|   ├── CameraSingleton
│   └── ZombieMono
│
├── Components
│   ├── BrainHealth
│   ├── BrainTag
│   ├── GraveyardProperties
│   ├── ZombieRiseRate
│   ├── ZombieWalkProperties
│   ├── ZombieEatProperties
│   ├── ZombieTimer
│   └── ZombieSpawnPoint
│
├── Aspects
│   ├── BrainAspect
│   ├── GraveyardAspect
│   ├── ZombieRiseAspect
│   ├── ZombieWalkAspect
│   └── ZombieEatAspect
│
├── Systems
│   ├── SpawnTombstoneSystem
│   ├── SpawnZombieSystem
│   ├── InitializeZombieSystem
│   ├── ZombieRiseSystem
│   ├── ZombieWalkSystem
│   ├── ZombieEatSystem
│   ├── ApplyBrainDamageSystem
│   └── CameraControllerSystem
│
└── MathHelpers
```

## Project Goal

هدف این پروژه تمرین و درک عملی مفاهیم **Unity ECS** و نحوه استفاده از آن در یک سناریوی واقعی بازی است.

این پروژه مفاهیمی مانند **Authoring/Baking، Components، Aspects، Systems، Jobs، Burst، Dynamic Buffers، EntityCommandBuffer و Enableable Components** را در قالب یک پروژه عملی پیاده‌سازی می‌کند.
