# GameAnalytics How To

## Initialisation

### Creating a GameAnalytics GameObject

Open the initial scene the game will load. Then select:

__*Window > GameAnalytics > Create GameAnalytics object*__
 
The GameAnalytics object will be placed in the scene you are currently on (remember to __save__ the scene to keep the changes).

Unity provides the GameObject methods called awake and start.
First all GameObjects get the awake call. When every awake is done then all GameObjects get the start call.

The execution order for each is __not fixed__.
The GameAnalytics settings GameObject is initialized in the awake method, but other GameObjects could have had their awake call happen before this.
Therefore when submitting events from GameObjects in Unity it is recommended to do this after (or inside) the start method. This will ensure everything is ready.
If an event is submitted before initialization then the log will output something like this:
*Warning/GameAnalytics: Could not add design event: Datastore not initialized*

### Custom ID

The SDK will __automatically generate__ a user id and this is perfectly fine for almost all cases.
Sometimes it is useful to supply this user_id manually – for example, if you download raw data for processing and need to match your internal user id (could be a database index on your user table) to the data collected through GameAnalytics.
Note that if you introduce this into a game that is already deployed (using the automatic id) it will start counting existing users as new users and your metrics will be affected. Use this from the start of the app lifetime.
Use the following piece of code to set the custom user id:

*GameAnalytics.SetCustomId("user1234567879");*

__Remember when using custom id you need to set the custom id before initializing GameAnalytics SDK or else it will not be used.__

From v3.11.0 and onwards you need to manually initialize the SDK by calling:

*GameAnalytics.Initialize();*

from your own GameObject (with script execution order coming after GameAnalytics script’s order if your object is in the same scene as the GameAnalytics object as some code is called on Awake event which needs to be called before initializing the sdk).

GameAnalytics supports 5 different types of __events__:

- Business
- Resource
- Progression
- Error
- Design

To send an event, remember to include the namespace GameAnalyticsSDK:

*using GameAnalyticsSDK;*

The next steps will guide you through the instrumentation of each of the different event types.

## Track real money transactions

With the Business event, you can include information on the specific type of in-app __item__ purchased, and where in the game the purchase was made. Additionally, the GameAnalytics SDK captures the app store __receipt__ to validate the purchases.

To add a business event call the following function:

### iOS
*GameAnalytics.NewBusinessEventIOS(string currency, int amount, string itemType, string itemId, string cartType, string receipt);*

### Android
*GameAnalytics.NewBusinessEventGooglePlay(string currency, int amount, string itemType, string itemId, string cartType, string receipt, string signature);*

## Balance virtual economy

Resources events is a way of tracking you __in-game economy__. From setting up the event you will be able to see three types of events in the tool:

- Flow, the __total balance__ from currency spend and rewarded
- Sink, is all __currency spend__ on items and lastly source being all currency rewarded in game

Here is some best practices for structuring the events:

*GameAnalytics.NewResourceEvent(GA_Resource.GAResourceFlowType.GAResourceFlowTypeSource, “Gems”, 400, “IAP”, “Coins400”);*

*GameAnalytics.NewResourceEvent(GA_Resource.GAResourceFlowType.GAResourceFlowTypeSink, “Gems”, 400, “Weapons”, “SwordOfFire”);*

*GameAnalytics.NewResourceEvent(GA_Resource.GAResourceFlowType.GAResourceFlowTypeSink, “Gems”, 100, “Boosters”, “BeamBooster5Pack”);*

*GameAnalytics.NewResourceEvent(GA_Resource.GAResourceFlowType.GAResourceFlowTypeSource, “BeamBooster”, 5, “Gems”, “BeamBooster5Pack”);*

*GameAnalytics.NewResourceEvent(GA_Resource.GAResourceFlowType.GAResourceFlowTypeSink, “BeamBooster”, 3, “Gameplay”, “BeamBooster5Pack”);*

Please note that any Resource currencies and Resource item types you want to use must first be __defined in Settings__, under the Setup tab.
Any value not defined will not be tracked in the events.

## Track player progression

Use this event to track when players __start and finish levels__ in your game. This event follows a 3 tier hierarchy structure to indicate a player's path or place in the game:

- World
- Level
- Phase

To add a progression event call the following function:

*GameAnalytics.NewProgressionEvent(GA_Progression.GAProgressionStatus progressionStatus, string progression01, string progression02, string progression03, int score);*

## Use custom design events

Track __any other__ concept in your game using this event type. For example, you could use this event to track GUI elements or tutorial steps.
Custom dimensions are __not__ supported on this event type.

To add a design event call the following function:

*GameAnalytics.NewDesignEvent (string eventName, float eventValue);*

## Log error events

You can use the Error event to log errors or warnings that players generate in your game. You can group the events by __severity level__ and attach a __message__, such as the stack trace.

To add a custom error event call the following function:

*GameAnalytics.NewErrorEvent (GAErrorSeverity severity, string message);*

## Use custom dimensions

Custom Dimensions can be used to __filter your data__ in the GameAnalytics webtool. To add custom dimensions to your events you will first have to create a __list__ of all the allowed values in Settings under the Setup tab.
Any value which is not defined will be ignored.

![Custom dimensions are beautiful!](https://s3.amazonaws.com/public.gameanalytics.com/resources/images/sdk_doc/wrapper_unity/custom_dimensions_and_resources.png "Custom dimensions")

For example, to set Custom dimension 01, call the following function:

*GameAnalytics.SetCustomDimension01(string customDimension);*

## Building

Unity shuold be able to take care of anything. If you encounter issues check the [documentation](https://gameanalytics.force.com/knowledgebase/s/article/Advanced-Integration-Unity-SDK#Platform-Build "GameAnalytics documentation").