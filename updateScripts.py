#!/usr/bin/env python

import os
import sys
import subprocess

from buildData import baseDestination, folderMapping

import argparse

PROJECT_ROOT, _ = os.path.split(os.path.realpath(__file__))
SCRIPT_DESTINATION = PROJECT_ROOT + '/' + baseDestination
SCRIPT_REPO_PATH = PROJECT_ROOT + '/.data/witches-scripts'

REPO_SERVER = "172.16.100.204"
REPO_NAME = "witches-scripts"
REPO_URL = "https://%s@github.com/VoltageEntertainment/witches-scripts.git"

DEFAULT_BRANCH = "master"

def main():
  parser = argparse.ArgumentParser()
  parser.add_argument('--user', action='store')
  parser.add_argument('--rev', action='store')
  parser.add_argument('--branch', action='store')
  args = parser.parse_args(sys.argv[1:])
  args = vars(args)

  user = args['user'] or os.environ.get('GIT_USER')
  rev = args['rev'] or os.environ.get('SCRIPT_REV')

  # if the user specifies a branch explicitly, it must reside in the branch prefix folder
  branch = args['branch'] or os.environ.get('GIT_BRANCH')
  if not branch:
    branch = DEFAULT_BRANCH

  if not user:
    print "No User found. Please specify the user either with --user or with the GIT_USER environment variable"
    sys.exit(1)

  updateRepository(user, rev, branch)

  checkRepoScripts(SCRIPT_REPO_PATH)

  prepareScriptFolders()
  syncScripts(SCRIPT_REPO_PATH, SCRIPT_DESTINATION)
  renameScriptFolders()


def updateRepository(user, revision, branch):
  ''' Pulls the latest files from the repository '''

  if not os.path.exists(SCRIPT_REPO_PATH):
    repoUrl = REPO_URL % user
    p = subprocess.Popen(["git", "clone", repoUrl, SCRIPT_REPO_PATH])
    p.wait()
    if p.returncode != 0:
      sys.exit(1)

  # get latest
  updateArgs = ["git", "fetch"]
  p = subprocess.Popen(updateArgs, cwd=SCRIPT_REPO_PATH)

  # git is either revision based, or branch based. Revision will trump branch
  checkoutArgs = ["git", "checkout"]
  if revision:
    checkoutArgs.append(revision)
  else:
    checkoutArgs.append(branch)

  p = subprocess.Popen(checkoutArgs, cwd=SCRIPT_REPO_PATH)
  p.wait()
  if p.returncode != 0:
    sys.exit(1)

def checkRepoScripts(newScriptFolder):
  missingFolders = []
  for folder in folderMapping:
    path = os.path.join(newScriptFolder, folder)
    if not os.path.isdir(path):
      missingFolders.append(path)

  if len(missingFolders) > 0:
    print "Error: The following scripts repo folders were not found: \n%s" % '\n'.join(missingFolders)
    print "The names in the repository may have changed. Please examine the repository at: %s" % newScriptFolder
    sys.exit(1)

def prepareScriptFolders():
  ''' Renames the existing script folders to match what the repository expects '''

  if not os.path.exists(SCRIPT_DESTINATION):
    os.makedirs(SCRIPT_DESTINATION)

  for initialName, renamedName in folderMapping.iteritems():
    path = SCRIPT_DESTINATION + '/' + renamedName
    newpath = SCRIPT_DESTINATION + '/' + initialName
    if os.path.exists(path):
      os.renames(path, newpath)

def syncScripts(newScriptFolder, scriptDestination):
  ''' Copies outdated scripts from the source folder to the destination folder (recursively) '''
  FOLDERS = [ getContainingFolder(svnFolder) for svnFolder in folderMapping ]
  
  includes = [ '--include=%s/**.json' % folder for folder in FOLDERS ]
  args = [ 'rsync', '-tazrC', '--include=*/' ]
  args += includes
  args += [ '--delete', "--exclude=*", '--prune-empty-dirs', newScriptFolder + '/', scriptDestination + '/' ]
  p = subprocess.Popen(args)
  p.wait()

def getContainingFolder(inputFolder):
  ''' Retrieves the most recent ancestor of the folder, or the folder itself if none exists '''
  lastIndex = inputFolder.rfind('/')
  if lastIndex == -1:
    return inputFolder
  return inputFolder[:lastIndex]

  
def renameScriptFolders():
  ''' Handles restoring 'client-friendly' names to the script folders '''
  for initialName, renamedName in folderMapping.iteritems():
    path = SCRIPT_DESTINATION + '/' + initialName
    newpath = SCRIPT_DESTINATION + '/' + renamedName
    
    if os.path.exists(path):
      os.renames(path, newpath)
    else:
      print 'path does not exist: %s' % path
  
if __name__ == '__main__':
  main()



