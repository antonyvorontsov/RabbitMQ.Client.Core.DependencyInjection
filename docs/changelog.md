# Changelog

All notable changes to this library will be documented in this file.

## [3.1.2] - 2020-01-31

### Added

- Ordering for message handlers.
- Separated the DI extensions file for message handlers so there are four of them now (each file covers each message handler type).
- Changed target platform to the .netstandard2.1

## [2.2.2] copy of [3.1.1] - 2020-01-18

**The latest version that supports backward compatibility for .Net Core 2.2**

### Added

- Backwards compability for .Net Core 2.2.

## [3.1.1] - 2020-01-18

### Added

- Pattern matching (`WildcardExtensions`) so message handlers can now
- Extension methods which allow user to set the exact exchange from which messages will be processed by message handlers.
- `MessageHandlingService` which is responsible for message processing.
- `WildcardExtensions` and `MessageHandlingService` unit tests.
- `AddRabbitMqClientTransient` extension methods.

## [2.2.1] copy of [3.1.0] - 2019-12-06

### Added

- Backwards compability for .Net Core 2.2.

## [3.1.0] - 2019-12-06

### Added

- Possibility to connect multiple RabbitMQ hosts and set connection names.

### Updated

- Version of the Net Core platform has been updated to the v3.1.

## [2.2.0] copy of [3.0.2] - 2019-11-04

### Added

- Backwards compability for .Net Core 2.2.

## [3.0.2] - 2019-11-04

### Added

- **!Breaking change** Boolean parameter `isConsuming` for `AddExchange` method.
- Extension methods for adding different exchanges (`AddProductionExchange` and `AddConsumptionExchange`) that wraps `isConsuming` parameter.

### Updated

- Way of binding exchanges and queues (not `isConsuming` value is being used).

### Changed

- Readme file.

## [3.0.1] - 2019-10-12

### Updated

- Updated libraries to the newer versions (Microsoft libraries, RabbitMQ.Client and Newtonsoft.Json).

## [3.0.0] - 2019-09-24

### Updated

- Version of the Net Core platform has been updated to the v3.0.

## [1.3.2] - 2019-08-27

### Added

- Possibility to configure RabbitMQ connection manually.

### Removed

- Some code redundancy in example projects.

## [1.3.1] - 2019-04-24

### Added

- First "stable" library release.
- Example projects that show how to configure RabbitMQ using library for message production and message consumption.