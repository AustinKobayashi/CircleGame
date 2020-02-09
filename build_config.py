import os

if os.path.exists(os.path.join("Builds", "fbapp-config.json")):
    os.remove(os.path.join("Builds", "fbapp-config.json"))

f = open(os.path.join("Builds", "fbapp-config.json"), "w")

f.write('{\n\t"instant_games": {\n\t\t"navigation_menu_version": "NAV_FLOATING",\n\t}\n}')
f.close()

f = open(os.path.join("Builds", "index.html"), "r")
index = f.read()
f.close()

if 'https://connect.facebook.net/en_US/fbinstant.6.3.js' not in index:
    start = index.index('<link rel="stylesheet" href="TemplateData/style.css">') + len('<link rel="stylesheet" href="TemplateData/style.css">')
    index = index[:start] + '\n\t<script src="https://connect.facebook.net/en_US/fbinstant.6.3.js"></script>\n' + index[start+1:]

if 'FBInstant' not in index:
    start = index.index('var unityInstance')
    r = start + 8

    while index[start : r] != '<script>':
        start -= 1
        r -= 1

    end = start
    l = end - 9

    while index[l : end] != '</script>':
        end += 1
        l += 1

    index = index[:start] + '''<script>FBInstant.initializeAsync().then(function() {
      var unityInstance = UnityLoader.instantiate("unityContainer", "Build/Builds.json", {onProgress: UnityProgress});
    });
    </script>''' + index[end:]

    os.remove(os.path.join("Builds", "index.html"))
    f = open(os.path.join("Builds", "index.html"), "w")
    f.write(index)
    f.close()


f = open(os.path.join("Builds", "TemplateData", "UnityProgress.js"), "r")
unity_progress = f.read()
f.close()

if 'FBInstant' not in unity_progress:
    cur = unity_progress.index('progress == 1')
    start = 0
    end = 0

    while unity_progress[cur] != ';':
        if unity_progress[cur] == ')':
            start = cur + 1
        cur += 1

    end = cur + 1

    unity_progress = unity_progress[:start] + '''{
    FBInstant.startGameAsync().then(function () {
      game.start();
      console.log("Game Started");
    });
    unityInstance.logo.style.display = unityInstance.progress.style.display = "none";
  }''' + unity_progress[end:]

    os.remove(os.path.join("Builds", "TemplateData", "UnityProgress.js"))
    f = open(os.path.join("Builds", "TemplateData", "UnityProgress.js"), "w")
    f.write(unity_progress)
    f.close()
