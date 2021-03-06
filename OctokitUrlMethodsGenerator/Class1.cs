﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net;
using Octokit;
using Octokit.Internal;

namespace OctokitUrlMethodsGenerator
{
    /// <summary>
    ///   Ensure input parameters
    /// </summary>
    internal static class Ensure
    {
        /// <summary>
        /// Checks an argument to ensure it isn't null.
        /// </summary>
        /// <param name = "value">The argument value to check</param>
        /// <param name = "name">The name of the argument</param>
        public static void ArgumentNotNull([ValidatedNotNull]object value, string name)
        {
            if (value != null) return;

            throw new ArgumentNullException(name);
        }

        /// <summary>
        /// Checks a string argument to ensure it isn't null or empty.
        /// </summary>
        /// <param name = "value">The argument value to check</param>
        /// <param name = "name">The name of the argument</param>
        public static void ArgumentNotNullOrEmptyString([ValidatedNotNull]string value, string name)
        {
            ArgumentNotNull(value, name);
            if (!string.IsNullOrWhiteSpace(value)) return;

            throw new ArgumentException("String cannot be empty", name);
        }

        /// <summary>
        /// Checks a timespan argument to ensure it is a positive value.
        /// </summary>
        /// <param name = "value">The argument value to check</param>
        /// <param name = "name">The name of the argument</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void GreaterThanZero([ValidatedNotNull]TimeSpan value, string name)
        {
            ArgumentNotNull(value, name);

            if (value.TotalMilliseconds > 0) return;

            throw new ArgumentException("Timespan must be greater than zero", name);
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class ValidatedNotNullAttribute : Attribute
    {
    }

    internal static class StringExtensions
    {
        public static bool IsBlank(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static bool IsNotBlank(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        public static Uri FormatUri(this string pattern, params object[] args)
        {

            return new Uri(string.Format(CultureInfo.InvariantCulture, pattern, args), UriKind.Relative);
        }

        public static string UriEncode(this string input)
        {
            return WebUtility.UrlEncode(input);
        }
    }

    static class EnumExtensions
    {
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        internal static string ToParameter(this Enum prop)
        {
            if (prop == null) return null;

            var propString = prop.ToString();
            var member = prop.GetType().GetMember(propString).FirstOrDefault();
            if (member == null) return null;

            var attribute = member.GetCustomAttributes(typeof(ParameterAttribute), false)
                .Cast<ParameterAttribute>()
                .FirstOrDefault();

            return attribute != null ? attribute.Value : propString.ToLowerInvariant();
        }
    }

    public class Class1
    {
        /// <summary>
        /// Returns the <see cref="Uri"/> that returns the assets specified by the asset id.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="id">The id of the release asset</param>
        /// <returns>The <see cref="Uri"/> that returns the assets specified by the asset id.</returns>
        public static Uri Asset(int repositoryId, int id)
        {
            return "repositories/{0}/releases/assets/{1}".FormatUri(repositoryId, id);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that returns all of the assignees to which issues may be assigned.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> that returns all of the assignees to which issues may be assigned.</returns>
        public static Uri Assignees(int repositoryId)
        {
            return "repositories/{0}/assignees".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for a specific blob.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> for a specific blob.</returns>
        public static Uri Blob(int repositoryId)
        {
            return Blob(repositoryId, "");
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for a specific blob.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="reference">The SHA of the blob</param>
        /// <returns>The <see cref="Uri"/> for a specific blob.</returns>
        public static Uri Blob(int repositoryId, string reference)
        {
            string blob = "repositories/{0}/git/blobs";
            if (!string.IsNullOrEmpty(reference))
            {
                blob += "/{1}";
            }
            return blob.FormatUri(repositoryId, reference);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that returns a 204 if the login belongs to an assignee of the repository. Otherwire returns a 404.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="login">The login for the user</param>
        /// <returns>The <see cref="Uri"/> that returns a 204 if the login belongs to an assignee of the repository. Otherwire returns a 404.</returns>
        public static Uri CheckAssignee(int repositoryId, string login)
        {
            return "repositories/{0}/assignees/{1}".FormatUri(repositoryId, login);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that returns a combined view of commit statuses for the specified reference.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="reference">The reference (SHA, branch name, or tag name) to list commits for</param>
        /// <returns>The <see cref="Uri"/> that returns a combined view of commit statuses for the specified reference.</returns>
        public static Uri CombinedCommitStatus(int repositoryId, string reference)
        {
            return "repositories/{0}/commits/{1}/status".FormatUri(repositoryId, reference);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for the specified commit.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="reference">The commit reference (SHA)</param>
        /// <returns>The <see cref="Uri"/> for the specified commit.</returns>
        public static Uri Commit(int repositoryId, string reference)
        {
            return "repositories/{0}/git/commits/{1}".FormatUri(repositoryId, reference);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for the specified comment.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="number">The comment number</param>
        /// <returns>The <see cref="Uri"/> for the specified comment.</returns>
        public static Uri CommitComment(int repositoryId, int number)
        {
            return "repositories/{0}/comments/{1}".FormatUri(repositoryId, number);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for the comments of a specified commit.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="sha">The sha of the commit</param>
        /// <returns>The <see cref="Uri"/> for the comments of a specified commit.</returns>
        public static Uri CommitComments(int repositoryId, string sha)
        {
            return "repositories/{0}/commits/{1}/comments".FormatUri(repositoryId, sha);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for the comments of a specified commit.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> for the comments of a specified commit.</returns>
        public static Uri CommitComments(int repositoryId)
        {
            return "repositories/{0}/comments".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that lists the commit statuses for the specified reference.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="reference">The reference (SHA, branch name, or tag name) to list commits for</param>
        /// <returns>The <see cref="Uri"/> that lists the commit statuses for the specified reference.</returns>
        public static Uri CommitStatuses(int repositoryId, string reference)
        {
            return "repositories/{0}/commits/{1}/statuses".FormatUri(repositoryId, reference);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for creating a commit object.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> for creating a commit object.</returns>
        public static Uri CreateCommit(int repositoryId)
        {
            return "repositories/{0}/git/commits".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> to use when creating a commit status for the specified reference.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="reference">The reference (SHA, branch name, or tag name) to list commits for</param>
        /// <returns>The <see cref="Uri"/> to use when creating a commit status for the specified reference.</returns>
        public static Uri CreateCommitStatus(int repositoryId, string reference)
        {
            return "repositories/{0}/statuses/{1}".FormatUri(repositoryId, reference);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for creating a merge object.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> for creating a merge object.</returns>
        public static Uri CreateMerge(int repositoryId)
        {
            return "repositories/{0}/merges".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for creating a tag object.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> for creating a tag object.</returns>
        public static Uri CreateTag(int repositoryId)
        {
            return "repositories/{0}/git/tags".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for the Deployments API for the given repository.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> for the Deployments API for the given repository.</returns>
        public static Uri Deployments(int repositoryId)
        {
            return "repositories/{0}/deployments".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for the Deployment Statuses API for the given deployment.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="deploymentId">Id of the deployment</param>
        /// <returns>The <see cref="Uri"/> for the Deployment Statuses API for the given deployment.</returns>
        public static Uri DeploymentStatuses(int repositoryId, int deploymentId)
        {
            return "repositories/{0}/deployments/{1}/statuses".FormatUri(repositoryId, deploymentId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for the specified issue.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="number">The issue number</param>
        /// <returns>The <see cref="Uri"/> for the specified issue.</returns>
        public static Uri Issue(int repositoryId, int number)
        {
            return "repositories/{0}/issues/{1}".FormatUri(repositoryId, number);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for the specified comment.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="id">The comment id</param>
        /// <returns>The <see cref="Uri"/> for the specified comment.</returns>
        public static Uri IssueComment(int repositoryId, int id)
        {
            return "repositories/{0}/issues/comments/{1}".FormatUri(repositoryId, id);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for the comments for all issues in a specific repo.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> for the comments for all issues in a specific repo.</returns>
        public static Uri IssueComments(int repositoryId)
        {
            return "repositories/{0}/issues/comments".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for the comments of a specified issue.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="number">The issue number</param>
        /// <returns>The <see cref="Uri"/> for the comments of a specified issue.</returns>
        public static Uri IssueComments(int repositoryId, int number)
        {
            return "repositories/{0}/issues/{1}/comments".FormatUri(repositoryId, number);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that returns the named label for the specified issue.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="number">The issue number</param>
        /// <param name="labelName">The name of the label</param>
        /// <returns>The <see cref="Uri"/> that returns the named label for the specified issue.</returns>
        public static Uri IssueLabel(int repositoryId, int number, string labelName)
        {
            return "repositories/{0}/issues/{1}/labels/{2}".FormatUri(repositoryId, number, labelName);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that returns all of the labels for the specified issue.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="number">The issue number</param>
        /// <returns>The <see cref="Uri"/> that returns all of the labels for the specified issue.</returns>
        public static Uri IssueLabels(int repositoryId, int number)
        {
            return "repositories/{0}/issues/{1}/labels".FormatUri(repositoryId, number);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for the specified issue to be locked/unlocked.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="number">The issue number</param>
        /// <returns>The <see cref="Uri"/> for the specified issue to be locked/unlocked.</returns>
        public static Uri IssueLock(int repositoryId, int number)
        {
            return "repositories/{0}/issues/{1}/lock".FormatUri(repositoryId, number);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that returns all of the issues for the currently logged in user specific to the repository.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> that returns all of the issues for the currently logged in user specific to the repository.</returns>
        public static Uri Issues(int repositoryId)
        {
            return "repositories/{0}/issues".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that returns the issue/pull request event and issue info for the specified event.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="id">The event id</param>
        /// <returns>The <see cref="Uri"/> that returns the issue/pull request event and issue info for the specified event.</returns>
        public static Uri IssuesEvent(int repositoryId, int id)
        {
            return "repositories/{0}/issues/events/{1}".FormatUri(repositoryId, id);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that returns the issue/pull request event info for the specified issue.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="number">The issue number</param>
        /// <returns>The <see cref="Uri"/> that returns the issue/pull request event info for the specified issue.</returns>
        public static Uri IssuesEvents(int repositoryId, int number)
        {
            return "repositories/{0}/issues/{1}/events".FormatUri(repositoryId, number);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that returns the issue/pull request event and issue info for the specified repository.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> that returns the issue/pull request event and issue info for the specified repository.</returns>
        public static Uri IssuesEvents(int repositoryId)
        {
            return "repositories/{0}/issues/events".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that returns the specified label.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="milestoneNumber">The milestone number</param>
        /// <returns>The <see cref="Uri"/> that returns the specified label.</returns>
        public static Uri Label(int repositoryId, string milestoneNumber)
        {
            return "repositories/{0}/labels/{1}".FormatUri(repositoryId, milestoneNumber);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that returns all of the labels for the specified repository.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> that returns all of the labels for the specified repository.</returns>
        public static Uri Labels(int repositoryId)
        {
            return "repositories/{0}/labels".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that returns the latest release for the specified repository
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> that returns the latest release for the specified repository</returns>
        public static Uri LatestRelease(int repositoryId)
        {
            return "repositories/{0}/releases/latest".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that returns the pull request merge state.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="number">The pull request number</param>
        /// <returns>The <see cref="Uri"/> that returns the pull request merge state.</returns>
        public static Uri MergePullRequest(int repositoryId, int number)
        {
            return "repositories/{0}/pulls/{1}/merge".FormatUri(repositoryId, number);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that returns the specified milestone.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="number">The milestone number</param>
        /// <returns>The <see cref="Uri"/> that returns the specified milestone.</returns>
        public static Uri Milestone(int repositoryId, int number)
        {
            return "repositories/{0}/milestones/{1}".FormatUri(repositoryId, number);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that returns all of the labels for all issues in the specified milestone.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="number">The milestone number</param>
        /// <returns>The <see cref="Uri"/> that returns all of the labels for all issues in the specified milestone.</returns>
        public static Uri MilestoneLabels(int repositoryId, int number)
        {
            return "repositories/{0}/milestones/{1}/labels".FormatUri(repositoryId, number);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that returns all of the milestones for the specified repository.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> that returns all of the milestones for the specified repository.</returns>
        public static Uri Milestones(int repositoryId)
        {
            return "repositories/{0}/milestones".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for the network of repositories.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> for the network of repositories.</returns>
        public static Uri NetworkEvents(int repositoryId)
        {
            return "networks/{0}/{1}/events".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that returns all of the notifications for the currently logged in user specific to the repository.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> that returns all of the notifications for the currently logged in user specific to the repository.</returns>
        public static Uri Notifications(int repositoryId)
        {
            return "repositories/{0}/notifications".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that returns the specified pull request.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="number">The pull request number</param>
        /// <returns>The <see cref="Uri"/> that returns the specified pull request.</returns>
        public static Uri PullRequest(int repositoryId, int number)
        {
            return "repositories/{0}/pulls/{1}".FormatUri(repositoryId, number);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that returns the commits on a pull request.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="number">The pull request number</param>
        /// <returns>The <see cref="Uri"/> that returns the commits on a pull request.</returns>
        public static Uri PullRequestCommits(int repositoryId, int number)
        {
            return "repositories/{0}/pulls/{1}/commits".FormatUri(repositoryId, number);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that returns the files on a pull request.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="number">The pull request number</param>
        /// <returns>The <see cref="Uri"/> that returns the files on a pull request.</returns>
        public static Uri PullRequestFiles(int repositoryId, int number)
        {
            return "repositories/{0}/pulls/{1}/files".FormatUri(repositoryId, number);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for the specified pull request review comment.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="number">The comment number</param>
        /// <returns>The <see cref="Uri"/> that </returns>
        public static Uri PullRequestReviewComment(int repositoryId, int number)
        {
            return "repositories/{0}/pulls/comments/{1}".FormatUri(repositoryId, number);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for the comments of a specified pull request review.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="number">The pull request number</param>
        /// <returns>The <see cref="Uri"/> that </returns>
        public static Uri PullRequestReviewComments(int repositoryId, int number)
        {
            return "repositories/{0}/pulls/{1}/comments".FormatUri(repositoryId, number);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for the pull request review comments on a specified repository.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> that </returns>
        public static Uri PullRequestReviewCommentsRepository(int repositoryId)
        {
            return "repositories/{0}/pulls/comments".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that lists the pull requests for a repository.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> that lists the pull requests for a repository.</returns>
        public static Uri PullRequests(int repositoryId)
        {
            return "repositories/{0}/pulls".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for the specified reference.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> for the specified reference.</returns>
        public static Uri Reference(int repositoryId)
        {
            return "repositories/{0}/git/refs".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for the specified reference.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="referenceName">The reference name</param>
        /// <returns>The <see cref="Uri"/> for the specified reference.</returns>
        public static Uri Reference(int repositoryId, string referenceName)
        {
            return "repositories/{0}/git/refs/{1}".FormatUri(repositoryId, referenceName);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that returns all the assets for the specified release for the specified repository.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="id">The id of the release</param>
        /// <returns>The <see cref="Uri"/> that returns all the assets for the specified release for the specified repository.</returns>
        public static Uri ReleaseAssets(int repositoryId, int id)
        {
            return "repositories/{0}/releases/{1}/assets".FormatUri(repositoryId, id);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that returns all of the releases for the specified repository.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> that returns all of the releases for the specified repository.</returns>
        public static Uri Releases(int repositoryId)
        {
            return "repositories/{0}/releases".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that returns a single release for the specified repository
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="id">The id of the release</param>
        /// <returns>The <see cref="Uri"/> that returns a single release for the specified repository</returns>
        public static Uri Releases(int repositoryId, int id)
        {
            return "repositories/{0}/releases/{1}".FormatUri(repositoryId, id);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for a repository branch.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="branchName">The name of the branch</param>
        /// <returns>The <see cref="Uri"/> for a repository branch.</returns>
        public static Uri RepoBranch(int repositoryId, string branchName)
        {
            return "repositories/{0}/branches/{1}".FormatUri(repositoryId, branchName);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that returns all of the branches for the specified repository.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> that returns all of the branches for the specified repository.</returns>
        public static Uri RepoBranches(int repositoryId)
        {
            return "repositories/{0}/branches".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that returns all of the collaborators for the specified repository.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> that returns all of the collaborators for the specified repository.</returns>
        public static Uri RepoCollaborators(int repositoryId)
        {
            return "repositories/{0}/collaborators".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for comparing two commits.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="base">The base commit</param>
        /// <param name="head">The head commit</param>
        /// <returns>The <see cref="Uri"/> for comparing two commits.</returns>
        public static Uri RepoCompare(int repositoryId, string @base, string head)
        {


            Ensure.ArgumentNotNullOrEmptyString(@base, "base");
            Ensure.ArgumentNotNullOrEmptyString(head, "head");
            var encodedBase = @base.UriEncode();
            var encodedHead = head.UriEncode();
            return "repositories/{0}/compare/{1}...{2}".FormatUri(repositoryId, encodedBase, encodedHead);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for a repository.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> for a repository.</returns>
        public static Uri Repository(int repositoryId)
        {
            return "repositories/{0}".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for getting an archive of a given repository's contents, in a specific format
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="archiveFormat">The format of the archive. Can be either tarball or zipball</param>
        /// <param name="reference">A valid Git reference.</param>
        /// <returns>The <see cref="Uri"/> for getting an archive of a given repository's contents, in a specific format</returns>
        public static Uri RepositoryArchiveLink(int repositoryId, ArchiveFormat archiveFormat, string reference)
        {
            return "repositories/{0}/{1}/{2}".FormatUri(repositoryId, archiveFormat.ToParameter(), reference);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for repository commits.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="reference">The commit reference (SHA)</param>
        /// <returns>The <see cref="Uri"/> for repository commits.</returns>
        public static Uri RepositoryCommit(int repositoryId, string reference)
        {
            return "repositories/{0}/commits/{1}".FormatUri(repositoryId, reference);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for repository commits.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> for repository commits.</returns>
        public static Uri RepositoryCommits(int repositoryId)
        {
            return "repositories/{0}/commits".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for getting the contents of the specified repository's root
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> for getting the contents of the specified repository's root</returns>
        public static Uri RepositoryContent(int repositoryId)
        {
            return "repositories/{0}/contents".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for getting the contents of the specified repository and path
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="path">The path of the contents to get</param>
        /// <returns>The <see cref="Uri"/> for getting the contents of the specified repository and path</returns>
        public static Uri RepositoryContent(int repositoryId, string path)
        {
            return "repositories/{0}/contents/{1}".FormatUri(repositoryId, path);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for getting the contents of the specified repository and path
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="path">The path of the contents to get</param>
        /// <param name="reference">The name of the commit/branch/tag. Default: the repository’s default branch (usually master)</param>
        /// <returns>The <see cref="Uri"/> for getting the contents of the specified repository and path</returns>
        public static Uri RepositoryContent(int repositoryId, string path, string reference)
        {
            return "repositories/{0}/contents/{1}?ref={2}".FormatUri(repositoryId, path, reference);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for repository contributors.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> for repository contributors.</returns>
        public static Uri RepositoryContributors(int repositoryId)
        {
            return "repositories/{0}/contributors".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for a deploy key for a repository
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="number">The id of the deploy key of the repository</param>
        /// <returns>The <see cref="Uri"/> for a deploy key for a repository</returns>
        public static Uri RepositoryDeployKey(int repositoryId, int number)
        {
            return "repositories/{0}/keys/{1}".FormatUri(repositoryId, number);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for deploy keys for a repository.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> for deploy keys for a repository.</returns>
        public static Uri RepositoryDeployKeys(int repositoryId)
        {
            return "repositories/{0}/keys".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that lists the repository forks for the specified reference.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> that lists the repository forks for the specified reference.</returns>
        public static Uri RepositoryForks(int repositoryId)
        {
            return "repositories/{0}/forks".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that gets the repository hook for the specified reference.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="hookId">The identifier of the repository hook</param>
        /// <returns>The <see cref="Uri"/> that gets the repository hook for the specified reference.</returns>
        public static Uri RepositoryHookById(int repositoryId, int hookId)
        {
            return "repositories/{0}/hooks/{1}".FormatUri(repositoryId, hookId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that can ping a specified repository hook
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="hookId">The identifier of the repository hook</param>
        /// <returns>The <see cref="Uri"/> that can ping a specified repository hook</returns>
        public static Uri RepositoryHookPing(int repositoryId, int hookId)
        {
            return "repositories/{0}/hooks/{1}/pings".FormatUri(repositoryId, hookId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that lists the repository hooks for the specified reference.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> that lists the repository hooks for the specified reference.</returns>
        public static Uri RepositoryHooks(int repositoryId)
        {
            return "repositories/{0}/hooks".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that can tests a specified repository hook
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="hookId">The identifier of the repository hook</param>
        /// <returns>The <see cref="Uri"/> that can tests a specified repository hook</returns>
        public static Uri RepositoryHookTest(int repositoryId, int hookId)
        {
            return "repositories/{0}/hooks/{1}/tests".FormatUri(repositoryId, hookId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for repository languages.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> for repository languages.</returns>
        public static Uri RepositoryLanguages(int repositoryId)
        {
            return "repositories/{0}/languages".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for getting the page metadata for a given repository
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> for getting the page metadata for a given repository</returns>
        public static Uri RepositoryPage(int repositoryId)
        {
            return "repositories/{0}/pages".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for getting all build metadata for a given repository
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> for getting all build metadata for a given repository</returns>
        public static Uri RepositoryPageBuilds(int repositoryId)
        {
            return "repositories/{0}/pages/builds".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for getting the build metadata for the last build for a given repository
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> for getting the build metadata for the last build for a given repository</returns>
        public static Uri RepositoryPageBuildsLatest(int repositoryId)
        {
            return "repositories/{0}/pages/builds/latest".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for getting the README of the specified repository
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> for getting the README of the specified repository</returns>
        public static Uri RepositoryReadme(int repositoryId)
        {
            return "repositories/{0}/readme".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for repository tags.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> for repository tags.</returns>
        public static Uri RepositoryTags(int repositoryId)
        {
            return "repositories/{0}/tags".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for repository teams.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> for repository teams.</returns>
        public static Uri RepositoryTeams(int repositoryId)
        {
            return "repositories/{0}/teams".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that lists the starred repositories for the authenticated user.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> that lists the starred repositories for the authenticated user.</returns>
        public static Uri Stargazers(int repositoryId)
        {
            return "repositories/{0}/stargazers".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that shows whether the repo is starred by the current user.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> that shows whether the repo is starred by the current user.</returns>
        public static Uri Starred(int repositoryId)
        {
            return "user/starred/{0}/{1}".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for the specified tag.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="reference">The tag reference (SHA)</param>
        /// <returns>The <see cref="Uri"/> for the specified tag.</returns>
        public static Uri Tag(int repositoryId, string reference)
        {
            return "repositories/{0}/git/tags/{1}".FormatUri(repositoryId, reference);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for the specified tree.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> for the specified tree.</returns>
        public static Uri Tree(int repositoryId)
        {
            return "repositories/{0}/git/trees".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for the specified tree.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="reference">The tree reference (SHA)</param>
        /// <returns>The <see cref="Uri"/> for the specified tree.</returns>
        public static Uri Tree(int repositoryId, string reference)
        {
            return "repositories/{0}/git/trees/{1}".FormatUri(repositoryId, reference);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for the specified tree.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <param name="reference">The tree reference (SHA)</param>
        /// <returns>The <see cref="Uri"/> for the specified tree.</returns>
        public static Uri TreeRecursive(int repositoryId, string reference)
        {
            return "repositories/{0}/git/trees/{1}?recursive=1".FormatUri(repositoryId, reference);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that shows whether the repo is starred by the current user.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> that shows whether the repo is starred by the current user.</returns>
        public static Uri Watched(int repositoryId)
        {
            return "repositories/{0}/subscription".FormatUri(repositoryId);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> that lists the watched repositories for the authenticated user.
        /// </summary>
        /// <param name="repositoryId">The ID of the repository</param>
        /// <returns>The <see cref="Uri"/> that lists the watched repositories for the authenticated user.</returns>
        public static Uri Watchers(int repositoryId)
        {
            return "repositories/{0}/subscribers".FormatUri(repositoryId);
        }


    }
}
