import json
import os
import datetime
from numpy import arange, linspace
from copy import deepcopy
import dpath.util
from subprocess import Popen
# to install modules run pip3 install numpy dpath


f = open("params_example_V2.json")
default_settings = json.load(f)
f.close()

settings = {
    "Transmission": ("virus/Transmission/Mean", [0.05, 0.15, 0.01]),
    "Lethality": ("virus/Lethality/Mean", [0.8, 1.0, 0.02])
}

for param in settings:
    print(param)

    new_settings = deepcopy(default_settings)
    nums = settings[param][1]
    interval = arange(nums[0], nums[1] + 1 if isinstance(nums[2],
                                                         int) else nums[1] + (nums[2]/10), nums[2])
    for val in interval:
        currentDT = datetime.datetime.now()
        print("\t" + str(round(val, 2)) + " Stamp: " + str(currentDT.hour) + ":" + str(currentDT.minute) + ":" + str(currentDT.second))
        new_settings["statisticsFolderExtension"] = str(
            param) + "/" + str(round(val, 2))
        dpath.util.set(new_settings, settings[param][0], round(val, 2))
        f = open("build\params.json", "w")
        json.dump(new_settings, f)
        f.close()
        for i in range(5):
            process = Popen(
                [".\AASMA_2020.exe", "-logFile", "log.txt", "-noGraphics", "-batchMode"], cwd=r'Build', shell=True)
            process.wait()
