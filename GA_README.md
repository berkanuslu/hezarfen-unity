# How To GameAnalytics

## Initialisation

From v3.11.0 and onwards you need to manually initialize the SDK by calling:

`GameAnalytics.Initialize();`

### Custom ID

The SDK will automatically generate a user id and this is perfectly fine for almost all cases.
Sometimes it is useful to supply this user_id manually – for example, if you download raw data for processing and need to match your internal user id (could be a database index on your user table) to the data collected through GameAnalytics.
Note that if you introduce this into a game that is already deployed (using the automatic id) it will start counting existing users as new users and your metrics will be affected. Use this from the start of the app lifetime.
Use the following piece of code to set the custom user id:

`GameAnalytics.SetCustomId("user1234567879");`

Remember when using custom id you need to set the custom id __before__ initializing GameAnalytics SDK or else it will not be used.

From your own GameObject (with script execution order coming after GameAnalytics script’s order if your object is in the same scene as the GameAnalytics object as some code is called on Awake event which needs to be called before initializing the sdk).

GameAnalytics supports 5 different types of events:

- Business
- Resource
- Progression
- Error
- Design

To send an event, remember to include the namespace GameAnalyticsSDK:

`using GameAnalyticsSDK;`

The next steps will guide you through the instrumentation of each of the different event types.

## Track real money transactions [[documentation](https://gameanalytics.force.com/knowledgebase/s/article/Event-Tracking-Unity-SDK#Business-Events "GameAnalytics.com")]

With the Business event, you can include information on the specific type of in-app item purchased, and where in the game the purchase was made. Additionally, the GameAnalytics SDK captures the app store receipt to __validate__ the purchases.
The method NewBusinessEventIOSAutoFetchReceipt will attempt to locate the latest receipt in iOS native code and submit the event if found.

To add a business event call the following functions:

```
// Android - Google Play
#if (UNITY_ANDROID)
    GameAnalytics.NewBusinessEventGooglePlay (string currency, int amount, string itemType, string itemId, string cartType, string receipt, string signature);
#endif

// iOS - with receipt
#if (UNITY_IOS)
    GameAnalytics.NewBusinessEventIOS (string currency, int amount, string itemType, string itemId, string cartType, string receipt);
#endif

// iOS - with autoFetchReceipt
#if (UNITY_IOS)
    GameAnalytics.NewBusinessEventIOSAutoFetchReceipt (string currency, int amount, string itemType, string itemId, string cartType);
#endif
```

### Business events without validation

It is also possible to send business events __without validation__:

`GameAnalytics.NewBusinessEvent (string currency, int amount, string itemType, string itemId, string cartType);`

## Balance virtual economy

__Resource events__ is a way of tracking you in-game economy. From setting up the event you will be able to see three types of events in the tool

- Flow, the __total balance__ from currency spend and rewarded
- Sink is all currency __spent__ on items and lastly source being all currency rewarded in game

Be careful to not call the resource event __too often__! In a game where the user collect coins fairly fast you should not call a Source event on each pickup.
Instead you should count the coins and send a single Source event when the user either completes or fails the level.

Here is some best practices for structuring the events:

```
GameAnalytics.NewResourceEvent(GA_Resource.GAResourceFlowType.GAResourceFlowTypeSource, “Gems”, 400, “IAP”, “Coins400”);
GameAnalytics.NewResourceEvent(GA_Resource.GAResourceFlowType.GAResourceFlowTypeSink, “Gems”, 400, “Weapons”, “SwordOfFire”);
GameAnalytics.NewResourceEvent(GA_Resource.GAResourceFlowType.GAResourceFlowTypeSink, “Gems”, 100, “Boosters”, “BeamBooster5Pack”);
GameAnalytics.NewResourceEvent(GA_Resource.GAResourceFlowType.GAResourceFlowTypeSource, “BeamBooster”, 5, “Gems”, “BeamBooster5Pack”);
GameAnalytics.NewResourceEvent(GA_Resource.GAResourceFlowType.GAResourceFlowTypeSink, “BeamBooster”, 3, “Gameplay”, “BeamBooster5Pack”);
```

Please note that any Resource currencies and Resource item types you want to use __must first be defined__ in Settings, under the Setup tab.
Any value not defined will not be tracked in the events.

## Track player progression

Use progression event to track when players start and finish __levels__ in your game. This event follows a 3 tier hierarchy structure (World, Level and Phase) to indicate a player's path or place in the game.

To add a progression event call the following function:

`GameAnalytics.NewProgressionEvent(GA_Progression.GAProgressionStatus progressionStatus, string progression01, string progression02, string progression03, int score);`

```
GameAnalytics.NewProgressionEvent (GAProgressionStatus.Start, "World1");
GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "World1", score);
GameAnalytics.NewProgressionEvent (GAProgressionStatus.Start, "World1", "Level1");
GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "World1", "Level1", score);
GameAnalytics.NewProgressionEvent (GAProgressionStatus.Start, "World1", "Level1", "Phase1");
GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "World1", "Level1", "Phase1", score);
```

## Log error events

You can use the Error event to log errors or warnings that players generate in your game. You can group the events by __severity level__ and attach a __message__, such as the stack trace.

To add a custom error event call the following function:

`GameAnalytics.NewErrorEvent (GAErrorSeverity severity, string message);`

## Use custom design events

Track __any other__ concept in your game using this event type. For example, you could use this event to track GUI elements or tutorial steps.

> Custom dimensions are not supported on this event type.

To add a design event call the following function:

`GameAnalytics.NewDesignEvent (string eventName, float eventValue);`

## Use custom dimensions

Custom Dimensions can be used to __filter__ your data in the GameAnalytics webtool. To add custom dimensions to your events you will first have to create a list of all the allowed values. You can do this in Settings under the Setup tab.
Any value which is not defined will be ignored.

![Custom dimensions are beautiful!](https://s3.amazonaws.com/public.gameanalytics.com/resources/images/sdk_doc/wrapper_unity/custom_dimensions_and_resources.png "Custom dimensions")


For example, to set Custom dimension 01, call the following function:

`GameAnalytics.SetCustomDimension01(string customDimension);`

## Custom Event Fields

It is possible to use a set of key-value pairs to add extra fields.

Here is an example of how to use it:

```
Dictionary<string, object> fields = new Dictionary<string, object>();
fields.put("test", 100);
fields.put("test_2", "hello_world");
```

> It will only be available through __raw data export__. For more information on custom event fields and raw data export go [here](https://gameanalytics.com/docs/s/article/Raw-Export-Overview "Raw export docs").

## Building

[See the documentation](https://gameanalytics.force.com/knowledgebase/s/article/Advanced-Integration-Unity-SDK#Platform-Build "GameAnalytics documentation")