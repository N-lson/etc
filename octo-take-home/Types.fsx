module Types

open System

type EnvironmentId = string

type Environment = {
    Id:EnvironmentId
    Name:string
}

type ProjectId = string
type Project = {
    Id:ProjectId
    Name:string
}

type Version = string option

type ReleaseId = string
type Release = {
    Id:ReleaseId
    ProjectId:ProjectId
    Version:Version
    Created:DateTimeOffset
}

type DeploymentId = string
type Deployment = {
    Id:DeploymentId
    ReleaseId:ReleaseId
    EnvironmentId:EnvironmentId
    DeployedAt:DateTimeOffset
}

type EnvironmentAndReleases = Environment * Release list