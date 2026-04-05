<template>
  <div v-if="isReady">
    <div v-if="!!message">{{ message }}</div>
    <div v-else>
      <div v-if="action == LogoutActions.Logout || action == LogoutActions.LogoutCallback">
        <loader />
      </div>
      <div v-else-if="action == LogoutActions.LoggedOut">{{message}}</div>
      <div v-else>Invalid action: {{action}}</div>
    </div>
  </div>
</template>
<script>
  import { AuthenticationResultStatus } from "../plugins/auth-service";
  import {
    QueryParameterNames,
    LogoutActions,
    ApplicationPaths,
  } from "../auth/ApiAuthorizationConstants";
  import Loader from '../components/Loader.vue'
  export default {
    props: ["action"],
    inject: ["isAuthenticated"],
    components: {
      Loader
    },
    mounted() {
      switch (this.action) {
        case LogoutActions.Logout:
          if (!!window.history.state.state.local) {
            this.logout(this.getReturnUrl());
          } else {
            // This prevents regular links to <app>/authentication/logout from triggering a logout
            this.isReady = true;
            this.message = "The logout was not initiated from within the page.";
          }
          break;
        case LogoutActions.LogoutCallback:
          this.processLogoutCallback();
          break;
        case LogoutActions.LoggedOut:
          this.isReady = true;
          this.message = "You successfully logged out!";
          break;
        default:
          throw new Error(`Invalid action '${this.action}'`);
      }
    },
    data() {
      return {
        message: undefined,
        isReady: false,
      };
    },
    methods: {
      async logout(returnUrl) {
        const state = { returnUrl };
        if (this.isAuthenticated.value) {
          const result = await this.$auth.signOut(state);
          switch (result.status) {
            case AuthenticationResultStatus.Redirect:
              break;
            case AuthenticationResultStatus.Success:
              await this.navigateToReturnUrl(returnUrl);
              break;
            case AuthenticationResultStatus.Fail:
              this.message = result.message;
              break;
            default:
              throw new Error("Invalid authentication result status.");
          }
        } else {
          this.message = "You successfully logged out!";
        }
      },

      async processLogoutCallback() {
        const url = window.location.href;
        const result = await this.$auth.completeSignOut(url);
        switch (result.status) {
          case AuthenticationResultStatus.Redirect:
            // There should not be any redirects as the only time completeAuthentication finishes
            // is when we are doing a redirect sign in flow.
            throw new Error("Should not redirect.");
          case AuthenticationResultStatus.Success:
            await this.navigateToReturnUrl(this.getReturnUrl(result.state));
            break;
          case AuthenticationResultStatus.Fail:
            this.message = result.message;
            break;
          default:
            throw new Error("Invalid authentication result status.");
        }
      },
      getReturnUrl(state) {
        const params = new URLSearchParams(window.location.search);
        const fromQuery = params.get(QueryParameterNames.ReturnUrl);
        if (fromQuery && !fromQuery.startsWith(`${window.location.origin}/`)) {
          // This is an extra check to prevent open redirects.
          throw new Error(
            "Invalid return url. The return url needs to have the same origin as the current page."
          );
        }
        return (
          (state && state.returnUrl) ||
          fromQuery ||
          `${window.location.origin}${ApplicationPaths.LoggedOut}`
        );
      },

      navigateToReturnUrl(returnUrl) {
        return window.location.replace(returnUrl);
      },
    },
  };
</script>
