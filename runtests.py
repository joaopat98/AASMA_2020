import json
import os
from numpy import arange, linspace
from copy import deepcopy
import dpath.util
from subprocess import Popen
# to install modules run pip3 install numpy dpath


f = open("params_example.json")
default_settings = json.load(f)
f.close()

settings = {
    "Boldness": ("government/boldness", [0.0, 0.9, 0.1]),
    "Trust": ("agentValues/Trust/Mean", [0.0, 0.9, 0.1])
}

for param in settings:
    print(param)

    new_settings = deepcopy(default_settings)
    nums = settings[param][1]
    interval = arange(nums[0], nums[1] + 1 if isinstance(nums[2],
                                                         int) else nums[1] + (nums[2]/10), nums[2])
    for val in interval:
        print("\t" + str(round(val, 2)))
        new_settings["statisticsFolderExtension"] = str(
            param) + "/" + str(round(val, 2))
        dpath.util.set(new_settings, settings[param][0], round(val, 2))
        f = open("Build\params.json", "w")
        json.dump(new_settings, f)
        f.close()
        for i in range(5):
            process = Popen(
                [".\AASMA_2020.exe", "-logFile", "log.txt", "-noGraphics", "-batchMode"], cwd=r'Build', shell=True)
            process.wait()
