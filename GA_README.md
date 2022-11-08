# How To GameAnalytics

[[documentation](https://gameanalytics.force.com/knowledgebase/s/article/Event-Tracking-Unity-SDK#Business-Events "GameAnalytics.com")] [[events](https://gameanalytics.force.com/knowledgebase/s/article/Introduction-to-Event-Types)]

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

## Events

GameAnalytics has 7 event types available for tracking different concepts in your game:

- [Business](#business-events): in-App Purchases supporting receipt validation on GA servers
- [Resource](#resource-events): managing the flow of virtual currencies – like gems or lives
- [Progression](#progression-events): level attempts with Start, Fail & Complete event
- [Error](#error-events): submit exception stack traces or custom error messages
- [Design](#design-events):  useful for tracking any of your games GUI elements
- [Ads](#ad-events): how players interact with ads within your game and monitor your ad performance
- [Impression](#impression-events): impression data taken from different ad networks, these are MoPub, Fyber and Ironsource

To send an event, remember to include the namespace GameAnalyticsSDK:

`using GameAnalyticsSDK;`

The next steps will guide you through the instrumentation of each of the different event types.

### Business Events

Use this event to track real money transactions in your game.
With the business event, you can include information on the specific type of in-app item purchased, and where in the game the purchase was made.
Additionally, the GameAnalytics SDK captures the app store receipt to __validate__ the purchases.
The method *NewBusinessEventIOSAutoFetchReceipt* will attempt to locate the latest receipt in iOS native code and submit the event if found.

> Purchase validation is supported only on iOS and Android at the moment.

#### Examples

- A player goes into the game’s shop and makes a purchase of a pack of coins
- You show the player a screen at the end of a level to prompt them to buy lives. They choose to purchase a pack of 3 lives that costs $0.99

| Field    | Examples   |           |
|----------|------------|-----------|
| cartType | shop       |endOfLevel |
| itemType | coinPack   |lives      |
| itemId   | coinPack2  |lifePack3  |
| amount   | 99         |23,2       |
| currency | USD        |USD        |

> We allow up to 10 cartTypes and up to 100 unique itemTypes and itemIds each.

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

#### Business events without validation

It is also possible to send business events __without validation__:

`GameAnalytics.NewBusinessEvent (string currency, int amount, string itemType, string itemId, string cartType)`

### Resource Events

This event can be used to track when players gain (source) or lose (sink) resources like virtual currency and lives.
__Resource events__ is a way of tracking you in-game economy. From setting up the event you will be able to see three types of events in the tool:

- Flow, the __total currency balance__
- Sink is all currency __spent__ on items
- Source is all currency __rewarded__ in game

#### Examples

- A player spends one life by starting a game
- A player buys a boost in the game with 100 gold currency
- A player earns a reward of 5 lives for watching an ad
- A player buys a pack of 100 gold currency as an in-app purchase for $0.99 from the store

| Field            | Examples   |              |         |          |
|------------------|------------|--------------|---------|----------|
| flowType         | sink       | sink         | source  | source   |
| itemType         | continuity | boost        | reward  | purchase |
| itemId           | startGame  | rainbowBoost | videoAd | goldPack |
| amount           | 1          | 100          | 5       | 100      |
| resourceCurrency | life       | gold         | life    | gold     |

Be careful to not call the resource event __too often__! In a game where the user collect coins fairly fast you should not call a Source event on each pickup.
Instead you should count the coins and send a single Source event when the user either completes or fails the level.

![schema](https://gameanalytics.force.com/knowledgebase/servlet/rtaImage?eid=ka06N000000KzCw&feoid=00N1t00000HE5Jr&refid=0EM6N000000TcoI)

`GameAnalytics.NewResourceEvent (GA_Resource.GAResourceFlowType flowType, string currency, float amount, string itemType, string itemId)`

Here is some best practices for structuring the events:

```
GameAnalytics.NewResourceEvent (GAResourceFlowType.Source, "Grenade", 2, "Looting", "BossKilled");
GameAnalytics.NewResourceEvent (GAResourceFlowType.Sink, "Grenade", 1, "Combat", "GrenadeThrow"); 
GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, “Gems”, 400, “IAP”, “Coins400”);
GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, “Gems”, 400, “Weapons”, “SwordOfFire”);
GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, “Gems”, 100, “Boosters”, “BeamBooster5Pack”);
GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, “BeamBooster”, 5, “Gems”, “BeamBooster5Pack”);
GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, “BeamBooster”, 3, “Gameplay”, “BeamBooster5Pack”);
```

Please note that any Resource currencies and Resource item types you want to use __must first be defined__ in Settings, under the Setup tab.
Any value not defined will not be tracked in the events.

### Progression Events

This event can be used to track when players __start and finish levels__ in your game. This event follows a 3 hierarchy structure (for example World, Level and Phase) to indicate a player’s path or place in the game.
“Progress” could mean things like levelling up, completing quests, completing missions, or completing milestones.
You can __only track one type of progression__ with this event: for example, if you have levels and quests in your game, the progression event should only be used for either tracking levels or quests, but not both.

However, do not worry! You can use Design Event and/or Custom Dimensions to track a secondary progression concept, if need be.

| Field             | Examples                    |
|-------------------|-----------------------------|
| progressionStatus | start - complete            |
| progression01     | level - quest               |
| progression02     | wildWest - enterCompetition |
| progression03     | day1 - europeCompetition    |
| value             | (blank)                     |

All three levels of the hierarchy structure (progression01, progression02 and progression03) OR you can send just progression01 and progression02 OR you can send just progression 01.

![schema](https://gameanalytics.force.com/knowledgebase/servlet/rtaImage?eid=ka06N000000KzCn&feoid=00N1t00000HE5Jr&refid=0EM6N000000TcoN)

#### Examples

- A players starts Day 1 of the Wild West world in Plants vs. Zombies 2
- A player completes the “Enter a Competition” quest in My Horse by entering the Europe competition

To add a progression event call the following function:

`GameAnalytics.NewProgressionEvent(GA_Progression.GAProgressionStatus progressionStatus, string progression01, string progression02, string progression03, int score)`

```
GameAnalytics.NewProgressionEvent (GAProgressionStatus.Start, "World1");
GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "World1", score);
GameAnalytics.NewProgressionEvent (GAProgressionStatus.Start, "World1", "Level1");
GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "World1", "Level1", score);
GameAnalytics.NewProgressionEvent (GAProgressionStatus.Start, "World1", "Level1", "Phase1");
GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, "World1", "Level1", "Phase1", score);
```

### Error events

You can use the Error event to log errors or warnings that players generate in your game. You can group the events by __severity level__ and attach a __message__, such as the stack trace.

| Field    | Examples       |
|----------|----------------|
| severity | Warning        |
| message  | crazyHighScore |

The options for severity are pre-defined in our SDK as:

- Info
- Debug
- Warning
- Error
- Critical

Some users choose to include a stack trace in the message field.
All error events are streamed in the real-time dashboard. They will also populate the widgets in the Quality dashboard once fully aggregated.

#### Examples

- A player has scored over 1 billion points on a level, which is not supposed to happen in your game.

To add a custom error event call the following function:

`GameAnalytics.NewErrorEvent (GAErrorSeverity severity, string message)`

`GameAnalytics.NewErrorEvent (GAErrorSeverity.Info, "Could not find available server.");`

### Design Events

You can use Design events to track __any concept__ in your game, for example, you could use this event to track GUI elements or tutorial steps.
> Custom dimensions are not supported on this event type.

| Field   | Examples                                |                    |
|---------|-----------------------------------------|--------------------|
| eventId | newUserTutorial:namedCharacter:complete | guiClick:volume:on |
| value   | (blank)                                 | (blank)            |

Fair Usage Policy
Event id’s are strings separated by colons defining an event hierarchy – like kill:robot:large.
It is important to not generate an excessive amount of unique nodes in the event hierarchy tree.
The maximum amount of unique nodes generated should not exceed 100,000 per day. With that in mind:
Poor Implementation Example
[level_name]:[weapon_used]:[damage_dealt]

Let's assume that:
level_name contains 100 values
weapon_used contains 300 values
damage_dealt contains 1-5000 values

This will generate an event hierarchy with: 100 * 300 * 5000 = 150M possible nodes. This is far too many.
Also in this example, damage_dealt should be included as a floating point value after the event string. More specific information on formatting Design events can be found within our integration guides here.
Game Blocking
Games exceeding this limit could be rate-limited, and experience other issues when using our core tool.

![schema](https://gameanalytics.force.com/knowledgebase/servlet/rtaImage?eid=ka06N000000KzD6&feoid=00N1t00000HE5Jr&refid=0EM6N000000TcoS)

Each level in the hierarchy will form a part in the name of the event. In order for our back-end to detect hierarchies, the event parts need to be separated by colons “:”.

For example, the composition of the first event in the scheme will be “Achievements:Killing:Neutral:10_Kills”, the “timePlayed” value is attached to the event separately as we can see the value “123” in the next code example.

`GameAnalytics.NewDesignEvent ("Achievement:Killing:Neutral:10_Kills", 123);`

Note : The 123 represents the “timePlayed” value. The entire hierarchy can be generated dynamically.

#### Examples

- A player completes the first step in your new user tutorial by naming their character
- A player clicks on the button to turn on the volume in your game

Track __any other__ concept in your game using this event type. For example, you could use this event to track GUI elements or tutorial steps.

> Custom dimensions are not supported on this event type.

To add a design event call the following function:

`GameAnalytics.NewDesignEvent (string eventName, float eventValue);`

#### Hierarchies

Hierarchies can have up to 5 levels. As a result only 5 event parts should be described in the string attached to the “NewDesignEvent” method of the SDK you are using. Next we propose a  hierarchy that covers the tracking of how many users have discovered a specific secret location where a secret weapon can be equipped.

![schema](https://gameanalytics.force.com/knowledgebase/servlet/rtaImage?eid=ka06N000000KzD6&feoid=00N1t00000HE5Jr&refid=0EM6N000000TcoX)

Similar to achievements, each unique design event needs to be triggered only once per user in order to track the number of users, not the amount of times the discovery of the secret weapons happened.

You may be wondering how this example implemented via code? Well here's an example of sending an event from this hierarchy via code:

`GameAnalytics.NewDesignEvent ("SecretWeapons:NuclearCannon");`

#### Funnels

Design Events can be included as steps inside funnels to track how many users have executed a particular sequence of actions across all sessions in a specified date range.

The lessons are counted using the 01, 02,.. counting method to make sure they are displayed in-order when using the event picker. The events need to be aggregated and also included in our database to be able to create a funnel using them. This can take up to two days, since we perform all the aggregations across all our games simultaneously.
Read more about the funnel feature in our dedicated article for Funnels.

#### Designing Custom Events

Design events are intended to provide deeper insights about the unique, custom actions players take in your game: those that are not a part of the core metrics and therefore not automatically tracked by GA.
They can be used to track multiple types of data in your game such as:
tutorial completion
battle results
clicks on the device screen
When instrumenting Design events, the event_id field is key as it essentially identifies the action that is happening. It should be unique for that particular event, in your game. It is important that an event_id describes the corresponding event as well as possible; this makes it easy to identify later when you have lots of events occurring.

##### Hierarchy Names

In order to obtain the correct visualizations on your metrics, it is important to develop hierarchies structuring the data into sets of categories and subcategories (and possibly several levels of these).

This is a best practice that will make it easier for you to identify, organize and work with your telemetry data.
Although there is freedom when choosing the different subcategories, consistency should be maintained across the different events once the order is chosen.
We recommend structuring your hierarchy as explained below:
[category]:[sub_category]:[outcome]
The first step in your hierarchy should be the category of events you are tracking. For example: Tutorial, PurchaseFlow, BattleName.
The second step is the point at which the action is occurring, or the particular step that is being taken.
The last item of the hierarchy should be the outcome of the event. This could be the action completed by the player, or the decision made by the player.

##### Event Limits

There are no limits to the amount of data your game can send to GameAnalytics. There is, however, a limit of 100,000 unique event_ids that GameAnalytics will process per game each day.
For example, you might have the following Design events to track users starting and completing the steps of a funnel:
Tutorial:Step01:Start
Tutorial:Step01:Complete
Tutorial:Step02:Start
Tutorial:Step02:Complete
Each of these is a unique event_id.
When designing event_id hierarchies it is a common mistake to nest events that have too many possible values. For example:
[monster type]:”kill”:[item id]
If [monster type] and [item id] have 1000 possible values, and all these occur in a day, then you have 1000*1000=1,000,000 unique events.
It is important to be careful when designing the event structure, so the game does not send too many unique events. In processing, GameAnalytics will start to ignore additional unique events if the number is above 100,000. This rarely happens, and is almost always due to games submitting dynamic content into the event string by mistake. This could be a position, value, monster id or some float value in the strings.

##### Concepts To Track

There are many concepts that you can track using Design events to understand player behaviour in your game. Some of the most common are as follows:
New user tutorial
Purchase funnel steps
UI clicks (e.g. clicking to enter the leaderboard)
Character selection
Ads watched in the game
Death type
Shares on a social network

### Ad Events

This event can be used to track when players interact with Ads within your game and monitor your ad __performance__.

| Field               | Examples                                                                |         |                                                                                            |   |
|---------------------|-------------------------------------------------------------------------|---------|--------------------------------------------------------------------------------------------|---|
| category            | Event category                                                          | string  | ads                                                                                        | Y |
| ad_sdk_name         | Name of the ad provider                                                 | string  | admob, fyber, applovin, ironsource, [any string]   Lowercase with no spaces or underscores | Y |
| ad_placement        | Placement/identifier of the ad within the game                          | string  | end_of_game, after_level, [any string]   Max 64 characters                                 | Y |
| ad_type             | Type of ad                                                              | string  | video \| rewarded_video \| playable \| interstitial \| offer_wall \| banner                | Y |
| ad_action           | The action made in relation to the ad                                   | string  | clicked \| show \| failed_show \| reward_received                                          | Y |
| ad_fail_show_reason | The reason why the ad failed to show                                    | string  | unknown \| offline \| no_fill \| internal_error \| invalid_request \| unable_to_precache   | N |
| ad_duration         | The duration in milliseconds that the ad was shown for                  | long    | 3500 (in milliseconds = 3.5 seconds)                                                       | N |
| ad_first            | The field can be added if this is the first ad to be shown for the user | boolean | TRUE                                                                                       | N |

#### Examples

- A player is shown an interstitial ad
- A player clicks on a rewarded ad
- An ad is failed to be shown to a player

The below table shows examples of how different fields can be used to track these:

| Field               | Examples     |                |              |   |
|---------------------|--------------|----------------|--------------|---|
| category            | ads          | ads            | ads          | Y |
| ad_sdk_name         | admob        | fyber          | ironsource   | Y |
| ad_placement        | end_of_level | ad_for_coins   | during_level | Y |
| ad_type             | interstitial | rewarded_video | banner       | Y |
| ad_action           | show         | clicked        | failed_show  | Y |
| ad_fail_show_reason | –            | –              | no_fill      | N |
| ad_duration         | 5000         | 6000           | –            | N |
| ad_first            | true         | –              | –            | N |

### Impression Events

Impression Events are used to track impression data from ads shown using different ad partners.
Currently we support MoPub and Fyber and we plan to add support for more providers in future. A key metric which is tracked is __Ad Revenue__.

| Field           | Examples   |                                                                                                                                                                                                                                                                                                                                                                                                                                       |                                                                                                                                             |
|-----------------|------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------|
| category        | String     | impression                                                                                                                                                                                                                                                                                                                                                                                                                            | Event category                                                                                                                              |
| ad_network_name | String     | mopub                                                                                                                                                                                                                                                                                                                                                                                                                                 | Name of ad network                                                                                                                          |
| impression_data | JsonObject | {   “adgroup_id”: “365cd2475e074026b93da14103a36b97”, “adgroup_name”: “Non-Mrect Ads”, “adgroup_priority”: 6, “adgroup_type”: “gtee”, “adunit_format”: “Fullscreen”, “adunit_id”: “24534e1901884e398f1253216226017e”, “adunit_name”: “Android Sample App Fullscreen”, “country”: “DK”, “currency”: “USD”, “id”: “0e6200add1eb4b8caadcf19c8314e394_00d45a3c00e5ce79”, “precision”: “publisher_defined”, “publisher_revenue”: 0.00005 } | Impression data could vary from each network. Description for MoPub data can be found here. Description for Fyber data can be found here.   |

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