<template>
<div>
    <div v-if="!!message">
        {{message}}
    </div>
    <div v-if="action == 'login'">
        <loader />
    </div>
    <div v-else-if="action == 'login-callback'">
        <loader />
    </div>
    <div v-if="action == 'profile' || action == 'register'">
        Profile/Register
    </div>
    <!-- <div v-else>
        Invalid Action
    </div> -->
</div>
</template>
<script>
  import { AuthenticationResultStatus } from '../plugins/auth-service';
  import { LoginActions, QueryParameterNames, ApplicationPaths } from '../auth/ApiAuthorizationConstants';
  import Loader from '../components/Loader.vue'

  export default {
    props: ["action"],
    data() {
      return {
        message: undefined,
      };
    },
    components: {
      Loader
    },
    methods: {
      async login(returnUrl) {
        const state = { returnUrl };
        const result = await this.$auth.signIn(state);
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
            throw new Error(`Invalid status result ${result.status}.`);
        }
      },
      async processLoginCallback() {
        const url = window.location.href;
        const result = await this.$auth.completeSignIn(url);
        switch (result.status) {
            case AuthenticationResultStatus.Redirect:
                // There should not be any redirects as the only time completeSignIn finishes
                // is when we are doing a redirect sign in flow.
                throw new Error('Should not redirect.');
            case AuthenticationResultStatus.Success:
                await this.navigateToReturnUrl(this.getReturnUrl(result.state));
                break;
            case AuthenticationResultStatus.Fail:
                this.message = result.message;
                break;
            default:
                throw new Error(`Invalid authentication result status '${result.status}'.`);
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
          (state && state.returnUrl) || fromQuery || `${window.location.origin}/`
        );
      },
      navigateToReturnUrl(returnUrl) {
        // It's important that we do a replace here so that we remove the callback uri with the
        // fragment containing the tokens from the browser history.
        window.location.replace(returnUrl);
    }
    },
    mounted() {
      const action = this.action;
      switch (action) {
        case 'login':
          this.login(this.getReturnUrl());
          break;
        case 'login-callback':
          this.processLoginCallback();
          break;
        case 'login-failed':
          const params = new URLSearchParams(window.location.search);
          const error = params.get(QueryParameterNames.Message);
          this.setState({ message: error });
          break;
        case 'profile':
          this.redirectToProfile();
          break;
        case 'register':
          this.redirectToRegister();
          break;
        default:
          throw new Error(`Invalid action '${action}'`);
      }
    },
  };
</script>