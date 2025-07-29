mergeInto(LibraryManager.library, {

  Hello: function () {
    window.alert("Hello, world!");
  },

  ShowAdv: function() {
    YaGames.init().then(ysdk => {
      ysdk.adv.showFullscreenAdv({
        callbacks: {
          onOpen: function() {
            SendMessage("Adv", "OnOpen");
          },
          onClose: function(wasShown) {
            SendMessage("Adv", "OnClose");
          },
          onError: function(error) {
            SendMessage("Adv", "OnError");
          },
          onOffline: function(error) {
            SendMessage("Adv", "OnOffline");
          }
        }
      });
    });
  },

  ShowReward: function(ptr) {
    var rewardType = UTF8ToString(ptr); // Преобразуем указатель из C# в строку JS

    ysdk.adv.showRewardedVideo({
      callbacks: {
        onOpen: function() {
          SendMessage("Adv", "OnOpenReward");
        },
        onRewarded: function() {
          SendMessage("Adv", "OnRewarded", rewardType);
        },
        onClose: function() {
          SendMessage("Adv", "OnCloseReward");
        },
        onError: function(e) {
          SendMessage("Adv", "OnErrorReward");
        }
      }
    });
  },

  GetLang: function(){
    var lang = ysdk.environment.i18n.lang;
    var bufferSize = lengthBytesUTF8(lang) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(lang, buffer, bufferSize);
    return buffer;
  },

  GameReady: function () {
    if (window.ysdk && ysdk.features && ysdk.features.LoadingAPI) {
      ysdk.features.LoadingAPI.ready();
      console.log("✅ Game is ready — ysdk.features.LoadingAPI.ready() called");
    } else {
      console.warn("⚠️ YSDK или LoadingAPI не инициализированы");
    }
  },

  StartGameplay: function () {
    if (window.ysdk && ysdk.features && ysdk.features.GameplayAPI) {
        ysdk.features.GameplayAPI.start();
        console.log("▶️ Yandex Gameplay started");
    } else {
        console.warn("⚠️ GameplayAPI не доступен");
    }
  },

  StopGameplay: function () {
    if (window.ysdk && ysdk.features && ysdk.features.GameplayAPI) {
        ysdk.features.GameplayAPI.stop();
        console.log("⏸ Yandex Gameplay stopped");
    } else {
        console.warn("⚠️ GameplayAPI не доступен");
    }
  }


});