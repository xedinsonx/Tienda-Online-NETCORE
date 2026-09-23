pipeline {
    agent {
        docker {
            image 'mcr.microsoft.com/dotnet/sdk:8.0'
        }
    }

    options {
        disableConcurrentBuilds()
        timestamps()
    }

    stages {
        stage('Restore') {
            steps {
                sh 'dotnet restore Tests/CrudDemoPro.Tests/CrudDemoPro.Tests.csproj'
            }
        }

        stage('Build') {
            steps {
                sh 'dotnet build Tests/CrudDemoPro.Tests/CrudDemoPro.Tests.csproj --configuration Release --no-restore'
            }
        }

        stage('Test') {
            steps {
                sh 'dotnet test Tests/CrudDemoPro.Tests/CrudDemoPro.Tests.csproj --configuration Release --no-build --logger "trx;LogFileName=test-results.trx" --collect:"XPlat Code Coverage"'
            }
        }

        stage('Publish') {
            steps {
                sh 'dotnet publish CrudDemoPro.csproj --configuration Release --no-build --output artifacts/publish'
            }
        }
    }

    post {
        always {
            junit testResults: '**/TestResults/*.trx', allowEmptyResults: true
            archiveArtifacts artifacts: 'artifacts/publish/**', allowEmptyArchive: true
        }
    }
}
